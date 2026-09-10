using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Infrastructure;

namespace InstitutoTriboDeDavi.API.BackgroundServices
{
    // Job diário de retenção: apaga os contadores de métrica com mais de N meses.
    // A tabela é agregada (1 linha por dia+chave), então já é pequena; isto só
    // garante que ela fique limitada para sempre. Roda de madrugada (Brasília).
    public class RetencaoMetricasHostedService : BackgroundService
    {
        private const int MesesRetencao = 24;
        private static readonly TimeSpan Horario = new(4, 0, 0);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RetencaoMetricasHostedService> _logger;

        public RetencaoMetricasHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<RetencaoMetricasHostedService> logger)
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
                    var repo = scope.ServiceProvider.GetRequiredService<IMetricaRepository>();
                    var removidos = await repo.LimparAnterioresAsync(limite);
                    if (removidos > 0)
                        _logger.LogInformation(
                            "Retenção de métricas: {Qtd} contador(es) anteriores a {Limite:d} removidos.",
                            removidos, limite);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha na retenção de métricas.");
                }
            }
        }
    }
}
