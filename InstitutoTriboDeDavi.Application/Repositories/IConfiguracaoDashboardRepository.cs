using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IConfiguracaoDashboardRepository
    {
        Task<ConfiguracaoDashboard> ObterPorUsuarioAsync(string usuarioLogin);
        Task<ConfiguracaoDashboard> SalvarAsync(ConfiguracaoDashboard configuracao);
    }
}
