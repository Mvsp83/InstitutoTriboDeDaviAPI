using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Solicitações internas entre a equipe (professor <-> administração).
    // Operacional — disponível a qualquer usuário autenticado, sem gate de
    // módulo comercial. A visibilidade é escopada no serviço pelo papel.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SolicitacaoInternaController : BaseController
    {
        private readonly ISolicitacaoInternaService _service;
        private readonly ILogger<SolicitacaoInternaController> _logger;

        public SolicitacaoInternaController(
            ISolicitacaoInternaService service,
            ILogger<SolicitacaoInternaController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // Caixa de entrada do usuário logado.
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return await ExecuteAsync(async () =>
            {
                var lista = await _service.Listar(UsuarioAutenticado);
                return Ok(new ResultViewModel
                {
                    Message = "Solicitações obtidas.",
                    Success = true,
                    Data = lista
                });
            });
        }

        // Total de não-resolvidas visíveis (para o contador/badge).
        [HttpGet("contador")]
        public async Task<IActionResult> Contador()
        {
            return await ExecuteAsync(async () =>
            {
                var total = await _service.ContarNaoResolvidas(UsuarioAutenticado);
                return Ok(new ResultViewModel
                {
                    Message = "Contador obtido.",
                    Success = true,
                    Data = new { naoResolvidas = total }
                });
            });
        }

        // Detalhe com a conversa.
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var solicitacao = await _service.Obter(id, UsuarioAutenticado);
                if (solicitacao == null)
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Solicitação não encontrada.",
                        Success = false,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Solicitação obtida.",
                    Success = true,
                    Data = solicitacao
                });
            });
        }

        // Abre uma solicitação (professor -> administração, ou admin -> professor).
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarSolicitacaoDTO dados)
        {
            return await ExecuteAsync(async () =>
            {
                var criada = await _service.Criar(dados, UsuarioAutenticado);
                _logger.LogInformation(
                    "Solicitação interna #{Id} aberta por {Login}.", criada.Id, UsuarioAutenticado.Login);
                return Ok(new ResultViewModel
                {
                    Message = "Solicitação enviada.",
                    Success = true,
                    Data = criada
                });
            });
        }

        // Responde na conversa.
        [HttpPost("{id}/responder")]
        public async Task<IActionResult> Responder(long id, [FromBody] ResponderSolicitacaoDTO dados)
        {
            return await ExecuteAsync(async () =>
            {
                var atualizada = await _service.Responder(id, dados?.Texto, UsuarioAutenticado);
                if (atualizada == null)
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Solicitação não encontrada ou sem acesso.",
                        Success = false,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Resposta enviada.",
                    Success = true,
                    Data = atualizada
                });
            });
        }

        // Muda o status (Aberta/Em andamento/Resolvida).
        [HttpPost("{id}/status")]
        public async Task<IActionResult> AlterarStatus(long id, [FromBody] AlterarStatusSolicitacaoDTO dados)
        {
            return await ExecuteAsync(async () =>
            {
                var atualizada = await _service.AlterarStatus(id, dados.Status, UsuarioAutenticado);
                if (atualizada == null)
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Solicitação não encontrada ou sem permissão.",
                        Success = false,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Status atualizado.",
                    Success = true,
                    Data = atualizada
                });
            });
        }
    }
}
