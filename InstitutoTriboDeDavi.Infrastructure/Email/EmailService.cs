using System.Net;
using System.Net.Mail;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace InstitutoTriboDeDavi.Infrastructure.Email
{
    // Envio de email via SMTP (System.Net.Mail). Funciona com Gmail (app
    // password), SendGrid SMTP, etc. Sem dependências extras.
    public class EmailService : IEmailService
    {
        private readonly SmtpConfig _config;

        public EmailService(IOptions<SmtpConfig> config)
        {
            _config = config.Value;
        }

        public async Task EnviarAsync(
            IEnumerable<string> destinatarios,
            string assunto,
            string corpoHtml)
        {
            if (string.IsNullOrWhiteSpace(_config.Host))
                throw new DomainException("Envio de email não configurado (Smtp:Host ausente).");

            var lista = (destinatarios ?? Enumerable.Empty<string>())
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Select(d => d.Trim())
                .Distinct()
                .ToList();

            if (lista.Count == 0)
                return;

            var remetente = string.IsNullOrWhiteSpace(_config.From) ? _config.User : _config.From;

            using var mensagem = new MailMessage
            {
                From = new MailAddress(remetente),
                Subject = assunto,
                Body = corpoHtml,
                IsBodyHtml = true
            };
            foreach (var destino in lista)
                mensagem.To.Add(destino);

            using var cliente = new SmtpClient(_config.Host, _config.Port)
            {
                EnableSsl = _config.EnableSsl,
                Credentials = new NetworkCredential(_config.User, _config.Password)
            };

            await cliente.SendMailAsync(mensagem);
        }
    }
}
