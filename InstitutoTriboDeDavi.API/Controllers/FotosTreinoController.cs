using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Fotos de treino: professor/admin postam 1 por turma/semana; admin modera;
    // o álbum público (site) lê só as publicadas. Gate no módulo "relacionamento".
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RequireModulo("relacionamento")]
    public class FotosTreinoController : BaseController
    {
        private readonly IFotoTreinoService _service;
        private readonly ILogger<FotosTreinoController> _logger;

        public FotosTreinoController(IFotoTreinoService service, ILogger<FotosTreinoController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // Postar (ou substituir) a foto da turma na semana. Professor usa o
        // próprio polo; admin pode informar o polo. Entra pendente de moderação.
        [HttpPost("upload")]
        [Authorize(Roles = "Administrador,Supervisor,Professor")]
        [RequestSizeLimit(15_000_000)] // ~15 MB (a compressão no cliente deixa bem menor)
        public async Task<IActionResult> Upload(
            IFormFile arquivo,
            [FromForm] DateTime dataAula,
            [FromForm] string categoria = "polo",
            [FromForm] int turma = 0,
            [FromForm] string legenda = "",
            [FromForm] long poloId = 0,
            [FromForm] bool consentimento = false)
        {
            return await ExecuteAsync(async () =>
            {
                if (arquivo == null || arquivo.Length == 0)
                    throw new DomainException("Nenhuma imagem enviada.");
                if (!(arquivo.ContentType ?? "").StartsWith("image/"))
                    throw new DomainException("O arquivo enviado não é uma imagem.");

                var cat = string.IsNullOrWhiteSpace(categoria) ? "polo" : categoria.Trim().ToLowerInvariant();
                var ehAdmin = UsuarioAutenticado.Role == UserRole.Administrador;

                long poloAlvo = 0;
                if (cat == "polo")
                {
                    poloAlvo = poloId > 0 ? poloId : (UsuarioAutenticado.PoloId ?? 0);
                    // Professor só posta no próprio polo; admin tem bypass.
                    ValidatePoloUsuario(poloAlvo);
                }
                else if (!ehAdmin)
                {
                    // Graduações/Geral/Eventos são coleções do admin.
                    throw new DomainException("Apenas o administrador pode postar nesta categoria.");
                }

                using var stream = arquivo.OpenReadStream();
                var dto = await _service.Postar(
                    cat, poloAlvo, turma, dataAula, legenda, UsuarioAutenticado.Id,
                    arquivo.FileName, arquivo.ContentType, stream, consentimento);

                return Ok(new ResultViewModel
                {
                    Message = dto.Publicada
                        ? "Foto publicada no álbum!"
                        : "Foto enviada! Ficará visível no site após a aprovação.",
                    Success = true,
                    Data = dto
                });
            });
        }

        // Lista para moderação (admin) — inclui as pendentes.
        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Listar()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Fotos obtidas com sucesso!",
                Success = true,
                Data = await _service.Listar()
            }));
        }

        [HttpPut("{id}/publicar")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Publicar(long id, [FromQuery] bool publicada)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.DefinirPublicacao(id, publicada);
                return Ok(new ResultViewModel
                {
                    Message = publicada ? "Foto publicada no álbum!" : "Foto retirada do álbum.",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Excluir(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Excluir(id);
                return Ok(new ResultViewModel
                {
                    Message = "Foto removida com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // Álbum público do site — só fotos aprovadas. Sem login.
        [HttpGet("publicas")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "publico")]
        public async Task<IActionResult> Publicas()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Álbum obtido com sucesso!",
                Success = true,
                Data = await _service.ListarPublicas()
            }));
        }

        // Stream da imagem — só das PUBLICADAS (tag <img> não envia token, então
        // pendentes não podem sair por aqui, por LGPD). Preview de pendente: /previa.
        [HttpGet("{id}/arquivo")]
        [AllowAnonymous]
        public async Task<IActionResult> Arquivo(long id, [FromQuery] bool mini = false)
        {
            var meta = await _service.Obter(id);
            if (meta == null || !meta.Publicada) return NotFound();

            var download = mini
                ? await _service.BaixarMiniatura(id)
                : await _service.BaixarArquivo(id);
            if (download == null) return NotFound();

            Response.Headers["Cache-Control"] = "public, max-age=86400";
            return File(download.Conteudo, download.ContentType);
        }

        // Preview para moderação (admin autenticado): devolve a imagem em base64
        // (data URI), pois a chamada carrega o token — serve para fotos pendentes.
        [HttpGet("{id}/previa")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Previa(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var dataUri = await _service.ObterPreviaDataUri(id);
                if (dataUri == null) return NotFound();
                return Ok(new ResultViewModel
                {
                    Message = "Prévia obtida.",
                    Success = true,
                    Data = new { dataUri }
                });
            });
        }

        // Config por polo: exigir ou não autorização para publicar (admin).
        [HttpGet("config-polos")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ConfigPolos()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Configuração obtida.",
                Success = true,
                Data = await _service.ListarConfigPolos()
            }));
        }

        [HttpPut("config-polos/{poloId}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> DefinirConfigPolo(long poloId, [FromQuery] bool requerAutorizacao)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.DefinirConfigPolo(poloId, requerAutorizacao);
                return Ok(new ResultViewModel
                {
                    Message = requerAutorizacao
                        ? "Fotos deste polo passarão por aprovação."
                        : "Fotos deste polo serão publicadas automaticamente.",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
