using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Common;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Corpo da denúncia (motivo é opcional).
    public class DenunciarRecadoRequest
    {
        public string Motivo { get; set; }
    }

    // Mural de recados (classificados da comunidade). Leitura para qualquer pessoa
    // logada — inclui o portal do responsável; criar/editar/remover é da equipe.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RequireModulo("relacionamento")]
    public class RecadoController : BaseController
    {
        private readonly IRecadoService _service;
        private readonly IFotoStorage _fotoStorage;
        private readonly ILogger<RecadoController> _logger;

        public RecadoController(IRecadoService service, IFotoStorage fotoStorage, ILogger<RecadoController> logger)
            : base(logger)
        {
            _service = service;
            _fotoStorage = fotoStorage;
            _logger = logger;
        }

        // Feed do mural: recados vigentes, visível a quem está logado (equipe e
        // portal do responsável). Roles explícitos para o token do responsável
        // também passar (e para não depender só do [Authorize] genérico).
        [HttpGet("mural")]
        [Authorize(Roles = "Administrador,Supervisor,Professor,Responsavel")]
        public async Task<IActionResult> Mural()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Mural obtido com sucesso!",
                Success = true,
                Data = await _service.ListarVigentes()
            }));
        }

        // Gestão da equipe: todos os recados (inclui expirados/inativos).
        [HttpGet("gerenciar")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Gerenciar()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Recados obtidos com sucesso!",
                Success = true,
                Data = await _service.ListarTodos()
            }));
        }

        [HttpPost("create")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Create([FromBody] RecadoDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Recado publicado com sucesso!",
                Success = true,
                Data = await _service.Create(dto, UsuarioAutenticado)
            }));
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] RecadoDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                // Foto antiga antes da alteração (Update valida o dono e lança se
                // não for). Se a foto mudou, remove o binário órfão (best-effort).
                var antigo = await _service.Obter(dto.Id);
                var atualizado = await _service.Update(dto, UsuarioAutenticado);

                if (!string.IsNullOrEmpty(antigo.FotoArquivoId) &&
                    antigo.FotoArquivoId != atualizado.FotoArquivoId)
                {
                    try { await _fotoStorage.ExcluirAsync(antigo.FotoArquivoId); }
                    catch { /* foto órfã não impede a atualização */ }
                }

                return Ok(new ResultViewModel
                {
                    Message = "Recado atualizado com sucesso!",
                    Success = true,
                    Data = atualizado
                });
            });
        }

        // Upload da foto do recado (opcional). Devolve o id do storage, que o
        // formulário inclui no create/update. Valida dimensões antes de guardar.
        [HttpPost("foto")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        [RequestSizeLimit(15_000_000)]
        public async Task<IActionResult> UploadFoto(IFormFile arquivo)
        {
            return await ExecuteAsync(async () =>
            {
                if (arquivo == null || arquivo.Length == 0)
                    throw new DomainException("Nenhuma imagem enviada.");
                if (!(arquivo.ContentType ?? "").StartsWith("image/"))
                    throw new DomainException("O arquivo enviado não é uma imagem.");

                using var buffer = new MemoryStream();
                await arquivo.CopyToAsync(buffer);
                var bytes = buffer.ToArray();
                Imagem.ValidarDimensoes(bytes); // guarda anti-bomba de descompressão

                using var stream = new MemoryStream(bytes);
                var fotoArquivoId = await _fotoStorage.UploadAsync(
                    arquivo.FileName, arquivo.ContentType, stream);

                return Ok(new ResultViewModel
                {
                    Message = "Foto recebida.",
                    Success = true,
                    Data = new { fotoArquivoId }
                });
            });
        }

        // Foto do recado como miniatura (data URI). Visível a quem vê o mural
        // (equipe + portal do responsável).
        [HttpGet("{id}/foto")]
        [Authorize(Roles = "Administrador,Supervisor,Professor,Responsavel")]
        public async Task<IActionResult> ObterFoto(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var recado = await _service.Obter(id);
                if (string.IsNullOrEmpty(recado.FotoArquivoId))
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Sem foto neste recado.",
                        Success = false,
                        Data = null
                    });

                var download = await _fotoStorage.BaixarAsync(recado.FotoArquivoId);
                using var ms = new MemoryStream();
                await download.Conteudo.CopyToAsync(ms);
                var mini = Imagem.GerarMiniatura(ms.ToArray(), maxLado: 480, qualidade: 72);
                var dataUri = $"data:image/jpeg;base64,{Convert.ToBase64String(mini)}";

                return Ok(new ResultViewModel
                {
                    Message = "Foto do recado.",
                    Success = true,
                    Data = new { dataUri }
                });
            });
        }

        // Denunciar um recado impróprio — qualquer logado (equipe ou portal).
        [HttpPost("{id}/denunciar")]
        [Authorize(Roles = "Administrador,Supervisor,Professor,Responsavel")]
        public async Task<IActionResult> Denunciar(long id, [FromBody] DenunciarRecadoRequest body)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Denunciar(id, body?.Motivo ?? string.Empty, UsuarioAutenticado.Login);
                return Ok(new ResultViewModel
                {
                    Message = "Denúncia registrada. A equipe vai analisar.",
                    Success = true,
                    Data = null
                });
            });
        }

        // Fila de denúncias pendentes — só a equipe.
        [HttpGet("denuncias")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Denuncias()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Denúncias obtidas com sucesso!",
                Success = true,
                Data = await _service.ListarDenunciasPendentes()
            }));
        }

        // Ignorar/encerrar uma denúncia (sem remover o recado).
        [HttpPost("denuncias/{denunciaId}/resolver")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> ResolverDenuncia(long denunciaId)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ResolverDenuncia(denunciaId, UsuarioAutenticado.Login);
                return Ok(new ResultViewModel
                {
                    Message = "Denúncia resolvida.",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                // Remove o binário da foto junto (best-effort), depois o registro.
                var recado = await _service.Obter(id);
                if (!string.IsNullOrEmpty(recado.FotoArquivoId))
                {
                    try { await _fotoStorage.ExcluirAsync(recado.FotoArquivoId); }
                    catch { /* foto órfã não impede remover o recado */ }
                }

                await _service.Delete(id, UsuarioAutenticado);
                return Ok(new ResultViewModel
                {
                    Message = "Recado removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
