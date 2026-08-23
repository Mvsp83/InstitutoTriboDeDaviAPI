using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Web Push: o front busca a chave pública, inscreve o dispositivo do usuário
    // logado e pode disparar um envio de teste. O envio real por eventos do
    // sistema fica a cargo dos serviços que chamarem IPushService.
    [ApiController]
    [Route("api/[controller]")]
    public class PushController : BaseController
    {
        private readonly IPushService _pushService;
        private readonly ILogger<PushController> _logger;

        public PushController(IPushService pushService, ILogger<PushController> logger) : base(logger)
        {
            _pushService = pushService;
            _logger = logger;
        }

        // Chave pública VAPID + se o push está configurado no servidor. Pública
        // porque o front precisa dela antes mesmo de decidir mostrar o opt-in.
        [HttpGet("vapid-public-key")]
        [AllowAnonymous]
        public IActionResult VapidPublicKey()
        {
            return Ok(new ResultViewModel
            {
                Message = "Chave pública obtida.",
                Success = true,
                Data = new
                {
                    publicKey = _pushService.ChavePublica,
                    configurado = _pushService.EstaConfigurado,
                }
            });
        }

        // Registra a inscrição do dispositivo para o usuário autenticado.
        [HttpPost("inscrever")]
        [Authorize]
        public async Task<IActionResult> Inscrever([FromBody] InscreverPushViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(model?.Endpoint) ||
                    string.IsNullOrWhiteSpace(model.P256dh) ||
                    string.IsNullOrWhiteSpace(model.Auth))
                {
                    return BadRequest(Responses.DomainErrorMessage("Dados da inscrição incompletos."));
                }

                await _pushService.InscreverAsync(
                    UsuarioAutenticado.Login, model.Endpoint, model.P256dh, model.Auth);

                return Ok(new ResultViewModel
                {
                    Message = "Notificações ativadas neste dispositivo.",
                    Success = true,
                    Data = null
                });
            });
        }

        // Remove a inscrição do dispositivo (usuário desativou as notificações).
        [HttpPost("desinscrever")]
        [Authorize]
        public async Task<IActionResult> Desinscrever([FromBody] DesinscreverPushViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                if (!string.IsNullOrWhiteSpace(model?.Endpoint))
                    await _pushService.DesinscreverAsync(model.Endpoint);

                return Ok(new ResultViewModel
                {
                    Message = "Notificações desativadas neste dispositivo.",
                    Success = true,
                    Data = null
                });
            });
        }

        // Envia uma notificação de teste para todos os dispositivos do usuário —
        // prova o caminho ponta-a-ponta depois de configurar as chaves VAPID.
        [HttpPost("testar")]
        [Authorize]
        public async Task<IActionResult> Testar()
        {
            return await ExecuteAsync(async () =>
            {
                var enviados = await _pushService.EnviarParaUsuarioAsync(
                    UsuarioAutenticado.Login,
                    "Tribo de Davi",
                    "Notificações estão funcionando! 🥋",
                    "/painel");

                return Ok(new ResultViewModel
                {
                    Message = enviados > 0
                        ? $"Enviado para {enviados} dispositivo(s)."
                        : "Nenhum dispositivo inscrito ou push não configurado.",
                    Success = true,
                    Data = new { enviados }
                });
            });
        }
    }
}
