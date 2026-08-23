using System;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Inscrição de alunos: recebe a ficha do site público e oferece a fila de
    // revisão para a equipe. Professor e Supervisor enxergam apenas o próprio
    // polo; Administrador vê todos.
    [ApiController]
    [Route("api/[controller]")]
    public class InscricaoController : BaseController
    {
        private readonly IInscricaoService _service;
        private readonly IPoloService _poloService;
        private readonly ILogger<InscricaoController> _logger;

        public InscricaoController(
            IInscricaoService service,
            IPoloService poloService,
            ILogger<InscricaoController> logger) : base(logger)
        {
            _service = service;
            _poloService = poloService;
            _logger = logger;
        }

        // Polos para montar o formulário público — só id e nome, nada mais.
        [HttpGet("polos")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.InscricaoRateLimit)]
        public async Task<IActionResult> PolosPublicos()
        {
            return await ExecuteAsync(async () =>
            {
                var polos = await _poloService.GetAll();
                var publicos = polos
                    .Select(p => new PoloPublicoDTO { Id = p.Id, Nome = p.Nome })
                    .OrderBy(p => p.Nome)
                    .ToList();

                return Ok(new ResultViewModel
                {
                    Message = "Polos obtidos com sucesso!",
                    Success = true,
                    Data = publicos
                });
            });
        }

        // Rematrícula: a família traz os dados de quem já é aluno para não
        // redigitar. Público, protegido por 2 fatores (CPF do responsável + data
        // de nascimento do aluno) e pelo rate limit. Data nula = não encontrado
        // (mesma resposta para evitar enumeração de CPFs).
        [HttpPost("buscar-aluno")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.InscricaoRateLimit)]
        public async Task<IActionResult> BuscarAluno([FromBody] ViewModels.Create.BuscarRematriculaViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                var dados = await _service.BuscarParaRematricula(
                    model?.CpfResponsavel ?? string.Empty,
                    model?.DataNascimento ?? default);

                return Ok(new ResultViewModel
                {
                    Message = dados != null
                        ? "Dados encontrados."
                        : "Não encontramos um aluno com esse CPF e data de nascimento.",
                    Success = true,
                    Data = dados
                });
            });
        }

        // Envio da ficha pelo responsável. Público por natureza: quem se
        // inscreve ainda não tem conta no sistema.
        [HttpPost("enviar")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.InscricaoRateLimit)]
        public async Task<IActionResult> Enviar([FromBody] InscricaoDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var resultado = await _service.Enviar(dto);

                // Não devolvemos a ficha inteira: a resposta é pública. O código
                // de acesso vai junto para a família guardar e usar no portal.
                return Ok(new ResultViewModel
                {
                    Message = "Inscrição enviada! A equipe do polo vai conferir os dados e entrar em contato.",
                    Success = true,
                    Data = new { resultado.Id, resultado.CodigoResponsavel }
                });
            });
        }

        // ── Fila de revisão ───────────────────────────────────────────────
        [HttpGet("fila")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Fila([FromQuery] int? status, [FromQuery] int? ano)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Inscrições obtidas com sucesso!",
                Success = true,
                Data = await _service.Listar(status, ano, PoloDoUsuario())
            }));
        }

        [HttpGet("pendentes/total")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> TotalPendentes()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Total obtido com sucesso!",
                Success = true,
                Data = await _service.ContarPendentes(PoloDoUsuario())
            }));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Obter(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var inscricao = await _service.Obter(id);
                ValidatePoloUsuario(inscricao.PoloId);

                return Ok(new ResultViewModel
                {
                    Message = "Inscrição obtida com sucesso!",
                    Success = true,
                    Data = inscricao
                });
            });
        }

        [HttpPost("{id}/aprovar")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Aprovar(long id, [FromBody] RevisaoInscricaoDTO revisao)
        {
            return await ExecuteAsync(async () =>
            {
                var inscricao = await _service.Obter(id);
                // Precisa poder mexer no polo de origem e também no de destino:
                // senão daria para "mover" um aluno para um polo alheio.
                ValidatePoloUsuario(inscricao.PoloId);
                if (revisao.PoloId > 0) ValidatePoloUsuario(revisao.PoloId);

                var matricula = await _service.Aprovar(id, revisao, UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Inscrição aprovada e aluno matriculado!",
                    Success = true,
                    Data = matricula
                });
            });
        }

        [HttpPost("{id}/recusar")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Recusar(long id, [FromBody] RevisaoInscricaoDTO revisao)
        {
            return await ExecuteAsync(async () =>
            {
                var inscricao = await _service.Obter(id);
                ValidatePoloUsuario(inscricao.PoloId);

                await _service.Recusar(id, revisao?.Observacao, UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Inscrição recusada.",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet("matriculas/{ano}")]
        [Authorize]
        public async Task<IActionResult> Matriculas(int ano)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Matrículas obtidas com sucesso!",
                Success = true,
                Data = await _service.ListarMatriculas(ano, PoloDoUsuario())
            }));
        }

        // Virada de ano: matricula em lote os alunos ativos ainda sem matrícula
        // no ano. Admin cobre todos os polos; professor/supervisor, só o seu.
        [HttpPost("matricular-ano/{ano}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> MatricularAno(int ano)
        {
            return await ExecuteAsync(async () =>
            {
                var r = await _service.MatricularAno(ano, PoloDoUsuario());
                return Ok(new ResultViewModel
                {
                    Message = r.Criadas > 0
                        ? $"{r.Criadas} aluno(s) matriculado(s) em {ano}."
                        : $"Todos os alunos já estavam matriculados em {ano}.",
                    Success = true,
                    Data = r
                });
            });
        }

        // Administrador enxerga todos os polos; os demais, só o seu.
        private long? PoloDoUsuario() =>
            UsuarioAutenticado.Role == UserRole.Administrador
                ? null
                : UsuarioAutenticado.PoloId;
    }
}
