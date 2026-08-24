using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Infrastructure;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace InstitutoTriboDeDavi.API.BackgroundServices
{
    // Job diário que envia por email os avisos dos eventos do calendário cuja
    // data de disparo (data - dias de antecedência) chegou. Roda no horário de
    // Brasília definido em Smtp:HorarioExecucao. A lógica de processamento fica
    // em INotificacaoCalendarioService (reutilizada pelo endpoint de teste).
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
                    using var scope = _scopeFactory.CreateScope();
                    var servico = scope.ServiceProvider.GetRequiredService<INotificacaoCalendarioService>();
                    await servico.ProcessarAsync(forcarEnvio: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao processar avisos do calendário.");
                }
            }
        }
    }
}
