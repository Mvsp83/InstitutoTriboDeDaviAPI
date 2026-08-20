using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace InstitutoTriboDeDavi.API.BackgroundServices
{
    // Job diário que envia por email os avisos dos eventos do calendário cuja
    // data de disparo (data - dias de antecedência) chegou. Roda no horário de
    // Brasília definido em Smtp:HorarioExecucao.
    public class NotificacaoCalendarioHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificacaoCalendarioHostedService> _logger;
        private readonly TimeSpan _horarioExecucao;

        public NotificacaoCalendarioHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificacaoCalendarioHostedService> logger,
            IOptions<SmtpConfig> config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            TimeSpan.TryParse(config.Value.HorarioExecucao, out _horarioExecucao);
            if (_horarioExecucao == default)
                _horarioExecucao = new TimeSpan(7, 0, 0);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Serviço de avisos do calendário iniciado. Execução diária às {Horario}.",
                _horarioExecucao.ToString(@"hh\:mm"));

            while (!stoppingToken.IsCancellationRequested)
            {
                var agora = FusoBrasil.Agora;
                var proxima = agora.Date.Add(_horarioExecucao);
                if (agora > proxima)
                    proxima = proxima.AddDays(1);

                var espera = proxima - agora;
                await Task.Delay(espera, stoppingToken);
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    await ProcessarAvisosAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao processar avisos do calendário.");
                }
            }
        }

        private async Task ProcessarAvisosAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IEventoCalendarioRepository>();
            var email = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var hoje = FusoBrasil.Agora.Date;
            var pendentes = await repo.ObterPendentesNotificacaoAsync();
            var enviados = 0;

            foreach (var evento in pendentes)
            {
                var dataEnvio = evento.Data.Date.AddDays(-evento.DiasAntecedencia);

                // Evento já passou sem notificar: marca para não reprocessar.
                if (hoje > evento.Data.Date)
                {
                    await repo.MarcarNotificadaAsync(evento.Id);
                    continue;
                }

                // Ainda não chegou a data de disparo.
                if (hoje < dataEnvio)
                    continue;

                var destinatarios = SepararEmails(evento.EmailsNotificacao);
                if (destinatarios.Count == 0)
                {
                    await repo.MarcarNotificadaAsync(evento.Id);
                    continue;
                }

                try
                {
                    await email.EnviarAsync(destinatarios, MontarAssunto(evento), MontarCorpo(evento));
                    await repo.MarcarNotificadaAsync(evento.Id);
                    enviados++;
                }
                catch (Exception ex)
                {
                    // Não marca como enviada: tenta de novo na próxima execução.
                    _logger.LogError(ex, "Falha ao enviar aviso do evento {Id}.", evento.Id);
                }
            }

            if (enviados > 0)
                _logger.LogInformation("{Total} aviso(s) do calendário enviado(s).", enviados);
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
