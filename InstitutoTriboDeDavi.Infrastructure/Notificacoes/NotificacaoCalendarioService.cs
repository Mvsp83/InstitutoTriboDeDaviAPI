using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InstitutoTriboDeDavi.Infrastructure.Notificacoes
{
    // Processa os avisos por email dos eventos do calendário. Usado tanto pelo
    // job diário (NotificacaoCalendarioHostedService) quanto pelo disparo
    // manual (endpoint de teste no EventoCalendarioController).
    public class NotificacaoCalendarioService : INotificacaoCalendarioService
    {
        private readonly IEventoCalendarioRepository _repo;
        private readonly IEmailService _email;
        private readonly ILogger<NotificacaoCalendarioService> _logger;

        public NotificacaoCalendarioService(
            IEventoCalendarioRepository repo,
            IEmailService email,
            ILogger<NotificacaoCalendarioService> logger)
        {
            _repo = repo;
            _email = email;
            _logger = logger;
        }

        public async Task<NotificacaoCalendarioResultado> ProcessarAsync(bool forcarEnvio = false)
        {
            var res = new NotificacaoCalendarioResultado();
            var hoje = FusoBrasil.Agora.Date;
            var pendentes = await _repo.ObterPendentesNotificacaoAsync();

            foreach (var evento in pendentes)
            {
                res.Processados++;
                var dataEnvio = evento.Data.Date.AddDays(-evento.DiasAntecedencia);

                // No modo automático (job), respeita a janela de data. No disparo
                // manual (forcarEnvio), envia todos os pendentes na hora.
                if (!forcarEnvio)
                {
                    if (hoje > evento.Data.Date)
                    {
                        // Evento já passou sem notificar: marca para não reprocessar.
                        await _repo.MarcarNotificadaAsync(evento.Id);
                        res.Ignorados++;
                        continue;
                    }
                    if (hoje < dataEnvio)
                    {
                        // Ainda não chegou a data de disparo.
                        res.Ignorados++;
                        continue;
                    }
                }

                var destinatarios = SepararEmails(evento.EmailsNotificacao);
                if (destinatarios.Count == 0)
                {
                    await _repo.MarcarNotificadaAsync(evento.Id);
                    res.Ignorados++;
                    res.Erros.Add($"Evento \"{evento.Titulo}\": sem destinatários de email.");
                    continue;
                }

                try
                {
                    await _email.EnviarAsync(destinatarios, MontarAssunto(evento), MontarCorpo(evento));
                    await _repo.MarcarNotificadaAsync(evento.Id);
                    res.Enviados++;
                }
                catch (Exception ex)
                {
                    // Não marca como enviada: tenta de novo na próxima execução.
                    _logger.LogError(ex, "Falha ao enviar aviso do evento {Id}.", evento.Id);
                    res.Erros.Add($"Evento \"{evento.Titulo}\": {ex.Message}");
                }
            }

            if (res.Enviados > 0)
                _logger.LogInformation("{Total} aviso(s) do calendário enviado(s).", res.Enviados);

            return res;
        }

        private static List<string> SepararEmails(string emails)
        {
            if (string.IsNullOrWhiteSpace(emails))
                return new List<string>();
            return emails
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToList();
        }

        private static string MontarAssunto(EventoCalendario e)
            => $"[Instituto Tribo de Davi] {e.Titulo} — {e.Data:dd/MM/yyyy}";

        private static string MontarCorpo(EventoCalendario e)
        {
            var periodo = e.DataFim.HasValue
                ? $"{e.Data:dd/MM/yyyy} a {e.DataFim.Value:dd/MM/yyyy}"
                : e.Data.ToString("dd/MM/yyyy");

            return $@"<div style='font-family:Segoe UI,Arial,sans-serif;color:#111;'>
  <h2 style='margin:0 0 8px 0;'>{e.Titulo}</h2>
  <p style='margin:0 0 4px 0;'><strong>Data:</strong> {periodo}</p>
  {(string.IsNullOrWhiteSpace(e.Descricao) ? "" : $"<p style='margin:8px 0;'>{e.Descricao}</p>")}
  <p style='margin-top:16px;color:#888;font-size:12px;'>Aviso automático do calendário do Instituto Tribo de Davi.</p>
</div>";
        }
    }
}
