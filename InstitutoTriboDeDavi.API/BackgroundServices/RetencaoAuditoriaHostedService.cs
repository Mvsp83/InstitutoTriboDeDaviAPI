using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Infrastructure;

namespace InstitutoTriboDeDavi.API.BackgroundServices
{
    // Job diário de retenção: apaga os logs de auditoria com mais de N meses,
    // mantendo a tabela limitada (custo de storage sob controle no plano free).
    // 24 meses cobre a prestação de contas anual com folga. Roda de madrugada
    // (Brasília), mesmo padrão da retenção de métricas.
    public class RetencaoAuditoriaHostedService : BackgroundService
    {
        private const int MesesRetencao = 24;
        private static readonly TimeSpan Horario = new(4, 15, 0);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RetencaoAuditoriaHostedService> _logger;

        public RetencaoAuditoriaHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<RetencaoAuditoriaHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var agora = FusoBrasil.Agora;
                var proxima = agora.Date.Add(Horario);
                if (agora > proxima)
                    proxima = proxima.AddDays(1);

                await Task.Delay(proxima - agora, stoppingToken);
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    var limite = FusoBrasil.Agora.Date.AddMonths(-MesesRetencao);
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ILogAuditoriaRepository>();
                    var removidos = await repo.LimparAnterioresAsync(limite);
                    if (removidos > 0)
                        _logger.LogInformation(
                            "Retenção de auditoria: {Qtd} log(s) anteriores a {Limite:d} removidos.",
                            removidos, limite);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha na retenção de auditoria.");
                }
            }
        }
    }
}
