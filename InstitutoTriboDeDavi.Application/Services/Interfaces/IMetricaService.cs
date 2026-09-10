using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IMetricaService
    {
        // Contabiliza um evento do site (beacon público). Eventos fora da lista
        // branca são ignorados (protege contra explosão de chaves/custo).
        Task RegistrarAsync(MetricaEventoDTO evento);
        // Resumo agregado dos últimos N dias para a tela do admin.
        Task<MetricaResumoDTO> ObterResumoAsync(int dias);
    }
}
