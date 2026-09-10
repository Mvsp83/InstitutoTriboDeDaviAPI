using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : BaseController
    {
        private readonly IAlunoService _alunoService;
        private readonly IFrequenciaService _frequenciaService;
        private readonly IAlunoFotoService _fotoService;
        private readonly ILogger<AlunoController> _logger;

        public AlunoController(IAlunoService alunoService, IPresencaService presencaService, IFrequenciaService frequenciaService, IAlunoFotoService fotoService, ILogger<AlunoController> logger) : base(logger)
        {
            _alunoService = alunoService;
            _frequenciaService = frequenciaService;
            _fotoService = fotoService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allAlunos = await _alunoService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Alunos encontrados com sucesso!",
                    Success = true,
                    Data = allAlunos
                });
            });
        }

        // Listagem paginada e enxuta (sem PII desnecessária) para a tela de
        // Alunos. Preferir este endpoint ao get-all: trafega menos dados e não
        // expõe CPF/RG/endereço na listagem.
        [HttpGet("lista")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ObterLista([FromQuery] AlunoListaFiltroDTO filtro)
        {
            return await ExecuteAsync(async () =>
            {
                var pagina = await _alunoService.ObterListaPaginadaAsync(filtro);

                return Ok(new ResultViewModel
                {
                    Message = "Alunos encontrados com sucesso!",
                    Success = true,
                    Data = pagina
                });
            });
        }

        // Público: número de "crianças atendidas" (alunos ativos, não
        // anonimizados) exibido no site. Só um agregado, sem dado pessoal.
        [HttpGet("total-publico")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "publico")]
        public async Task<IActionResult> TotalPublico()
        {
            return await ExecuteAsync(async () =>
            {
                var total = await _alunoService.ObterTotalAtendidosAsync();

                return Ok(new ResultViewModel
                {
                    Message = "Total de crianças atendidas.",
                    Success = true,
                    Data = new { total }
                });
            });
        }

        // Ficha completa de um aluno (todos os campos), para exibição/edição sob
        // demanda — assim a listagem pode ser enxuta. Admin acessa qualquer um;
        // professor/supervisor só os do próprio polo.
        [HttpGet("{id:long}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var aluno = await _alunoService.Get(id);

                if (aluno == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                ValidatePoloUsuario(aluno.PoloId); // admin bypass; professor só o seu polo

                return Ok(new ResultViewModel
                {
                    Message = "Aluno encontrado com sucesso!",
                    Success = true,
                    Data = aluno
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Create([FromBody] AlunoDTO alunoDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var alunoCreated = await _alunoService.Create(alunoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno criado com sucesso!",
                    Success = true,
                    Data = alunoCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] AlunoDTO alunoDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var alunoUpdated = await _alunoService.Update(alunoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno atualizado com sucesso!",
                    Success = true,
                    Data = alunoUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var aluno = await _alunoService.Get(id);

                if (aluno == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _alunoService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet("alunos-mais-faltantes")]
        [Authorize]
        public async Task<IActionResult> GetAlunosFaltas()
        {
            return await ExecuteAsync(async () =>
            {
                var totalAlunos = await _frequenciaService.GetAlunosFaltasAsync(UsuarioAutenticado);

                if (!totalAlunos.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Alunos obtidos com sucesso!",
                    Success = true,
                    Data = totalAlunos
                });
            });
        }

        [HttpGet("get-por-polo")]
        [Authorize]
        public async Task<IActionResult> ObterAlunos([FromQuery] List<int> turmas)
        {
            return await ExecuteAsync(async () =>
            {
                var allAlunos = await _alunoService.ObterAlunosPorTurmaAsync(UsuarioAutenticado, turmas);

                if (!allAlunos.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Alunos obtidos com sucesso!",
                    Success = true,
                    Data = allAlunos
                });
            });
        }

        [HttpGet("pendentes")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> ObterPendentes()
        {
            return await ExecuteAsync(async () =>
            {
                var pendentes = await _alunoService.ObterAlunosPendentesAsync(UsuarioAutenticado);

                if (!pendentes.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum aluno pendente de turma encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = $"{pendentes.Count} aluno(s) aguardando atribuição de turma.",
                    Success = true,
                    Data = pendentes
                });
            });
        }

        [HttpPatch("atribuir-turma")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> AtribuirTurma([FromBody] AtribuirTurmaDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var aluno = await _alunoService.AtribuirTurmaAsync(dto);

                return Ok(new ResultViewModel
                {
                    Message = $"Turma {dto.Turma} atribuída ao aluno '{aluno.Nome}' com sucesso!",
                    Success = true,
                    Data = aluno
                });
            });
        }

        // ── LGPD (art. 18) ────────────────────────────────────────────────

        // Direito de acesso/portabilidade: baixa tudo que o sistema guarda
        // sobre o aluno num único pacote legível.
        [HttpGet("{id}/exportar-dados")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ExportarDados(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var dados = await _alunoService.ExportarDadosAsync(id, UsuarioAutenticado?.Login);

                if (dados == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Dados do aluno reunidos com sucesso!",
                    Success = true,
                    Data = dados
                });
            });
        }

        // Direito de eliminação: apaga os dados pessoais do aluno mantendo os
        // registros operacionais anonimizados (prestação de contas).
        [HttpPost("{id}/anonimizar")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Anonimizar(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var aluno = await _alunoService.AnonimizarAsync(id);

                if (aluno == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                _logger.LogWarning("Dados pessoais do aluno #{Id} anonimizados por {Usuario}.", id, UsuarioAutenticado?.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Dados pessoais do aluno anonimizados com sucesso!",
                    Success = true,
                    Data = aluno
                });
            });
        }

        // Apoio à política de retenção: lista alunos sem atividade há mais de
        // 'mesesInativo' meses — candidatos à eliminação.
        [HttpGet("candidatos-retencao")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> CandidatosRetencao([FromQuery] int mesesInativo = 18)
        {
            return await ExecuteAsync(async () =>
            {
                if (mesesInativo < 1)
                    mesesInativo = 1;

                var candidatos = await _alunoService.ObterCandidatosRetencaoAsync(mesesInativo);

                return Ok(new ResultViewModel
                {
                    Message = $"{candidatos.Count} aluno(s) sem atividade há mais de {mesesInativo} meses.",
                    Success = true,
                    Data = candidatos
                });
            });
        }

        // ── Código de acesso do responsável ─────────────────────────────────

        [HttpGet("{id}/codigo-responsavel")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> ObterCodigoResponsavel(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var codigo = await _alunoService.ObterCodigoResponsavelAsync(id);

                return Ok(new ResultViewModel
                {
                    Message = codigo == null ? "Nenhum código gerado ainda." : "Código obtido.",
                    Success = true,
                    Data = new { codigo }
                });
            });
        }

        // (Re)gera o código e o devolve para o admin compartilhar com a família.
        [HttpPost("{id}/codigo-responsavel")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> GerarCodigoResponsavel(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var codigo = await _alunoService.GerarCodigoResponsavelAsync(id);

                if (codigo == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Código de acesso gerado com sucesso!",
                    Success = true,
                    Data = new { codigo }
                });
            });
        }

        // Impressão em lote: garante um código para cada aluno do escopo e
        // devolve a lista para imprimir e entregar às famílias.
        [HttpPost("codigos-responsavel/preparar")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> PrepararCodigosResponsavel()
        {
            return await ExecuteAsync(async () =>
            {
                var lista = await _alunoService.PrepararCodigosResponsavelAsync(UsuarioAutenticado);

                return Ok(new ResultViewModel
                {
                    Message = $"{lista.Count} código(s) prontos para impressão.",
                    Success = true,
                    Data = lista
                });
            });
        }

        // ── Foto do aluno ──────────────────────────────────────────────────
        [HttpPost("{id}/foto")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        [RequestSizeLimit(15_000_000)]
        public async Task<IActionResult> SalvarFoto(long id, IFormFile arquivo)
        {
            return await ExecuteAsync(async () =>
            {
                if (arquivo == null || arquivo.Length == 0)
                    throw new DomainException("Nenhuma imagem enviada.");
                if (!(arquivo.ContentType ?? "").StartsWith("image/"))
                    throw new DomainException("O arquivo enviado não é uma imagem.");

                var poloId = await _fotoService.ObterPoloId(id);
                if (poloId == null) throw new DomainException("Aluno não encontrado.");
                ValidatePoloUsuario(poloId.Value);

                using var stream = arquivo.OpenReadStream();
                await _fotoService.SalvarFoto(id, arquivo.FileName, arquivo.ContentType, stream);

                return Ok(new ResultViewModel { Message = "Foto atualizada!", Success = true, Data = null });
            });
        }

        [HttpDelete("{id}/foto")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> RemoverFoto(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var poloId = await _fotoService.ObterPoloId(id);
                if (poloId == null) throw new DomainException("Aluno não encontrado.");
                ValidatePoloUsuario(poloId.Value);

                await _fotoService.RemoverFoto(id);
                return Ok(new ResultViewModel { Message = "Foto removida.", Success = true, Data = null });
            });
        }

        // Foto em base64 (a tag <img> não envia token). Acesso: admin, professor
        // do polo, ou o responsável do próprio aluno.
        [HttpGet("{id}/foto")]
        [Authorize]
        public async Task<IActionResult> ObterFoto(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var poloId = await _fotoService.ObterPoloId(id);
                if (poloId == null) return NotFound();

                var ehResponsavel = User.FindFirst("AlunoId")?.Value == id.ToString();
                if (!ehResponsavel)
                    ValidatePoloUsuario(poloId.Value); // admin bypass; professor precisa ser do polo

                var dataUri = await _fotoService.ObterFotoDataUri(id);
                if (dataUri == null) return NotFound();

                return Ok(new ResultViewModel { Message = "Foto obtida.", Success = true, Data = new { dataUri } });
            });
        }

        // Config global de onde a foto do aluno aparece (leitura p/ todas as telas).
        [HttpGet("config-foto")]
        [Authorize]
        public async Task<IActionResult> ObterConfigFoto()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Config obtida.",
                Success = true,
                Data = await _fotoService.ObterConfig()
            }));
        }

        [HttpPut("config-foto")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> SalvarConfigFoto([FromBody] ConfiguracaoFotoAlunoDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                await _fotoService.SalvarConfig(dto);
                return Ok(new ResultViewModel { Message = "Configuração salva!", Success = true, Data = null });
            });
        }
    }
}
