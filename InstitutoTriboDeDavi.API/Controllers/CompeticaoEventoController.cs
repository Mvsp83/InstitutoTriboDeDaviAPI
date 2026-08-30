using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Competições (eventos) que o instituto acompanha, com os atletas
    // participantes e seus resultados. Gerido pela equipe.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
    public class CompeticaoEventoController : BaseController
    {
        private readonly ICompeticaoEventoService _service;
        private readonly ILogger<CompeticaoEventoController> _logger;

        public CompeticaoEventoController(
            ICompeticaoEventoService service,
            ILogger<CompeticaoEventoController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        private static IActionResult Sucesso(object data, string msg = "OK") =>
            new OkObjectResult(new ResultViewModel { Message = msg, Success = true, Data = data });

        [HttpGet]
        public Task<IActionResult> Listar() =>
            ExecuteAsync(async () => Sucesso(await _service.Listar(), "Competições."));

        [HttpGet("{id}")]
        public Task<IActionResult> Obter(long id) =>
            ExecuteAsync(async () =>
            {
                var e = await _service.Obter(id);
                if (e == null)
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Competição não encontrada.",
                        Success = false,
                        Data = null
                    });
                return Sucesso(e, "Competição.");
            });

        [HttpPost]
        public Task<IActionResult> Criar([FromBody] CompeticaoEventoDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.Criar(dto), "Competição cadastrada."));

        [HttpPut]
        public Task<IActionResult> Atualizar([FromBody] CompeticaoEventoDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.Atualizar(dto), "Competição atualizada."));

        [HttpDelete("{id}")]
        public Task<IActionResult> Remover(long id) =>
            ExecuteAsync(async () => { await _service.Remover(id); return Sucesso(null, "Competição removida."); });

        // ── Participações ────────────────────────────────────────────────────

        [HttpPost("{id}/participacoes")]
        public Task<IActionResult> AdicionarParticipacao(long id, [FromBody] ParticipacaoAtletaDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.AdicionarParticipacao(id, dto), "Participante adicionado."));

        [HttpPut("participacoes")]
        public Task<IActionResult> AtualizarParticipacao([FromBody] ParticipacaoAtletaDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.AtualizarParticipacao(dto), "Resultado atualizado."));

        [HttpDelete("participacoes/{id}")]
        public Task<IActionResult> RemoverParticipacao(long id) =>
            ExecuteAsync(async () => { await _service.RemoverParticipacao(id); return Sucesso(null, "Participante removido."); });
    }
}
