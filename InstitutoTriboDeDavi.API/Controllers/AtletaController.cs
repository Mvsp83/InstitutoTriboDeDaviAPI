using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Atletas de alto rendimento: perfil (aluno selecionado do cadastro),
    // índices/avaliações, competições, diário e metas. Gerido pela equipe.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
    public class AtletaController : BaseController
    {
        private readonly IAtletaService _service;
        private readonly ILogger<AtletaController> _logger;

        public AtletaController(IAtletaService service, ILogger<AtletaController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        private static IActionResult Sucesso(object data, string msg = "OK") =>
            new OkObjectResult(new ResultViewModel { Message = msg, Success = true, Data = data });

        [HttpGet]
        public Task<IActionResult> Listar() =>
            ExecuteAsync(async () => Sucesso(await _service.Listar(), "Atletas."));

        [HttpGet("{id}")]
        public Task<IActionResult> Obter(long id) =>
            ExecuteAsync(async () =>
            {
                var atleta = await _service.Obter(id);
                if (atleta == null)
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Atleta não encontrado.",
                        Success = false,
                        Data = null
                    });
                return Sucesso(atleta, "Atleta.");
            });

        [HttpPost]
        public Task<IActionResult> Criar([FromQuery] long alunoId) =>
            ExecuteAsync(async () => Sucesso(await _service.Criar(alunoId), "Atleta incluído."));

        [HttpPut]
        public Task<IActionResult> Atualizar([FromBody] AtletaDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.AtualizarPerfil(dto), "Perfil atualizado."));

        [HttpDelete("{id}")]
        public Task<IActionResult> Remover(long id) =>
            ExecuteAsync(async () => { await _service.Remover(id); return Sucesso(null, "Atleta removido."); });

        // ── Avaliações (índices) ─────────────────────────────────────────────

        [HttpPost("{id}/avaliacoes")]
        public Task<IActionResult> AdicionarAvaliacao(long id, [FromBody] AvaliacaoFisicaDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.AdicionarAvaliacao(id, dto), "Avaliação registrada."));

        [HttpDelete("avaliacoes/{id}")]
        public Task<IActionResult> RemoverAvaliacao(long id) =>
            ExecuteAsync(async () => { await _service.RemoverAvaliacao(id); return Sucesso(null, "Avaliação removida."); });

        // ── Competições ──────────────────────────────────────────────────────

        [HttpPost("{id}/competicoes")]
        public Task<IActionResult> AdicionarCompeticao(long id, [FromBody] CompeticaoDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.AdicionarCompeticao(id, dto), "Competição registrada."));

        [HttpDelete("competicoes/{id}")]
        public Task<IActionResult> RemoverCompeticao(long id) =>
            ExecuteAsync(async () => { await _service.RemoverCompeticao(id); return Sucesso(null, "Competição removida."); });

        // ── Diário ───────────────────────────────────────────────────────────

        [HttpPost("{id}/anotacoes")]
        public Task<IActionResult> AdicionarAnotacao(long id, [FromBody] AnotacaoAtletaDTO dto) =>
            ExecuteAsync(async () =>
                Sucesso(await _service.AdicionarAnotacao(id, dto?.Texto, UsuarioAutenticado.Login), "Anotação salva."));

        [HttpDelete("anotacoes/{id}")]
        public Task<IActionResult> RemoverAnotacao(long id) =>
            ExecuteAsync(async () => { await _service.RemoverAnotacao(id); return Sucesso(null, "Anotação removida."); });

        // ── Metas ────────────────────────────────────────────────────────────

        [HttpPost("{id}/metas")]
        public Task<IActionResult> AdicionarMeta(long id, [FromBody] MetaAtletaDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.AdicionarMeta(id, dto), "Meta criada."));

        [HttpPost("metas/{id}/status")]
        public Task<IActionResult> AlterarStatusMeta(long id, [FromQuery] int status) =>
            ExecuteAsync(async () => Sucesso(await _service.AlterarStatusMeta(id, status), "Meta atualizada."));

        [HttpDelete("metas/{id}")]
        public Task<IActionResult> RemoverMeta(long id) =>
            ExecuteAsync(async () => { await _service.RemoverMeta(id); return Sucesso(null, "Meta removida."); });

        // ── Lesões ───────────────────────────────────────────────────────────

        [HttpPost("{id}/lesoes")]
        public Task<IActionResult> AdicionarLesao(long id, [FromBody] LesaoDTO dto) =>
            ExecuteAsync(async () => Sucesso(await _service.AdicionarLesao(id, dto), "Lesão registrada."));

        [HttpPost("lesoes/{id}/recuperada")]
        public Task<IActionResult> MarcarLesaoRecuperada(long id, [FromQuery] bool recuperado) =>
            ExecuteAsync(async () => Sucesso(await _service.MarcarLesaoRecuperada(id, recuperado), "Lesão atualizada."));

        [HttpDelete("lesoes/{id}")]
        public Task<IActionResult> RemoverLesao(long id) =>
            ExecuteAsync(async () => { await _service.RemoverLesao(id); return Sucesso(null, "Lesão removida."); });
    }
}
