using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IMetricaRepository
    {
        // Incrementa (ou cria) o contador do dia para a chave informada.
        Task IncrementarAsync(DateTime data, string chave, long delta);
        // Todas as métricas a partir de uma data (para o resumo do admin).
        Task<List<MetricaDiaria>> ObterDesdeAsync(DateTime desde);
        // Retenção: apaga contadores anteriores ao limite. Devolve quantos.
        Task<int> LimparAnterioresAsync(DateTime limite);
    }
}
