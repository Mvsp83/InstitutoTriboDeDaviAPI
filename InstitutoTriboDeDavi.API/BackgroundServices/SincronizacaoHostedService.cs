using InstitutoTriboDeDavi.Infrastructure;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace InstitutoTriboDeDavi.API.BackgroundServices
{
    public class SincronizacaoHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SincronizacaoHostedService> _logger;
        private readonly TimeSpan _horarioExecucao;

        public SincronizacaoHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<SincronizacaoHostedService> logger,
            IOptions<GoogleSheetsConfig> config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _horarioExecucao = TimeSpan.Parse(config.Value.HorarioExecucao);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Serviço de sincronização iniciado. Execução diária às {Horario}.",
                _horarioExecucao.ToString(@"hh\:mm"));

            while (!stoppingToken.IsCancellationRequested)
            {
                // Horário de Brasília, independente do relógio do servidor
                // (em nuvem o padrão é UTC — 02:00 viraria 23:00 no Brasil)
                var agora = FusoBrasil.Agora;
                var proximaExecucao = agora.Date.Add(_horarioExecucao);

                // Se o horário de hoje já passou, agenda para amanhã
                if (agora > proximaExecucao)
                    proximaExecucao = proximaExecucao.AddDays(1);

                var espera = proximaExecucao - agora;

                _logger.LogInformation(
                    "Próxima sincronização agendada para {ProximaExecucao} (em {Horas}h {Minutos}min).",
                    proximaExecucao.ToString("dd/MM/yyyy HH:mm"),
                    (int)espera.TotalHours,
                    espera.Minutes);

                await Task.Delay(espera, stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                _logger.LogInformation("Iniciando sincronização automática com Google Sheets...");

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IGoogleSheetsService>();
                    var resultados = await service.SincronizarTodasAsPlanilhasAsync("Automatico");

                    var totalInseridos = resultados.Sum(r => r.Inseridos);
                    var totalAtualizados = resultados.Sum(r => r.Atualizados);
                    var totalErros = resultados.Sum(r => r.Erros.Count);

                    _logger.LogInformation(
                        "Sincronização concluída: {Inseridos} inseridos, {Atualizados} atualizados, {Erros} erros.",
                        totalInseridos, totalAtualizados, totalErros);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado na sincronização automática.");
                }
            }
        }
    }
}
