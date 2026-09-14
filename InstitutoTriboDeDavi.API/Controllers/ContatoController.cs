using System.Net;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // "Fale Conosco": recebe mensagens do site público (anônimo, com rate limit)
    // e oferece a caixa de entrada para a equipe (Administrador).
    [ApiController]
    [Route("api/[controller]")]
    public class ContatoController : BaseController
    {
        private readonly IMensagemContatoService _service;
        private readonly IEmailService _emailService;
        private readonly SmtpConfig _smtp;
        private readonly ILogger<ContatoController> _logger;

        public ContatoController(
            IMensagemContatoService service,
            IEmailService emailService,
            IOptions<SmtpConfig> smtp,
            ILogger<ContatoController> logger) : base(logger)
        {
            _service = service;
            _emailService = emailService;
            _smtp = smtp.Value;
            _logger = logger;
        }

        [HttpPost("enviar")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.InscricaoRateLimit)]
        public async Task<IActionResult> Enviar([FromBody] EnviarContatoDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var salva = await _service.Enviar(dto);
                if (salva != null)
                    await NotificarPorEmailAsync(salva);

                return Ok(new ResultViewModel
                {
                    Message = "Mensagem enviada! Em breve entraremos em contato.",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Listar()
        {
            return await ExecuteAsync(async () =>
            {
                var mensagens = await _service.Listar();
                return Ok(new ResultViewModel
                {
                    Message = "Mensagens obtidas!",
                    Success = true,
                    Data = mensagens
                });
            });
        }

        [HttpGet("nao-lidas")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> NaoLidas()
        {
            return await ExecuteAsync(async () =>
            {
                var total = await _service.ContarNaoLidas();
                return Ok(new ResultViewModel
                {
                    Message = "Total de não lidas.",
                    Success = true,
                    Data = new { total }
                });
            });
        }

        [HttpPut("marcar-lida/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> MarcarLida(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.MarcarLida(id);
                return Ok(new ResultViewModel { Message = "Marcada como lida.", Success = true, Data = null });
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Excluir(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Excluir(id);
                return Ok(new ResultViewModel { Message = "Mensagem removida.", Success = true, Data = null });
            });
        }

        // Notificação best-effort: a mensagem já está salva; se o SMTP não estiver
        // configurado ou falhar, apenas registra e segue (não quebra o envio).
        private async Task NotificarPorEmailAsync(MensagemContatoDTO m)
        {
            var destino = string.IsNullOrWhiteSpace(_smtp.From) ? _smtp.User : _smtp.From;
            if (string.IsNullOrWhiteSpace(destino) || string.IsNullOrWhiteSpace(_smtp.Host))
                return;

            try
            {
                string E(string v) => WebUtility.HtmlEncode(v ?? string.Empty);
                var corpo =
                    $"<p><strong>{E(m.Nome)}</strong></p>" +
                    $"<p>E-mail: {E(m.Email)}<br/>Telefone: {E(m.Telefone)}</p>" +
                    (string.IsNullOrWhiteSpace(m.Assunto) ? "" : $"<p><strong>Assunto:</strong> {E(m.Assunto)}</p>") +
                    $"<p>{E(m.Mensagem).Replace("\n", "<br/>")}</p>";

                var assunto = string.IsNullOrWhiteSpace(m.Assunto)
                    ? "Fale Conosco — nova mensagem"
                    : $"Fale Conosco — {m.Assunto}";

                await _emailService.EnviarAsync(new[] { destino }, assunto, corpo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao notificar mensagem de contato por e-mail (segue salva).");
            }
        }
    }
}
