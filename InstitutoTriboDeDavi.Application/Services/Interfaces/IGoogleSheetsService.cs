using InstitutoTriboDeDavi.Application.Import;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IGoogleSheetsService
    {
        //Task<List<ImportacaoResultado>> SincronizarTodasAsPlanilhasAsync();
        //Task<ImportacaoResultado> SincronizarPlanilhaAsync(long poloId);

        Task<List<ImportacaoResultado>> SincronizarTodasAsPlanilhasAsync(string origem = "Automatico");
        Task<ImportacaoResultado> SincronizarPlanilhaAsync(long poloId, string origem = "Manual");
    }
}
