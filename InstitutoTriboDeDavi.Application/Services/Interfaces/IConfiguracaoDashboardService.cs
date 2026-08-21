using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IConfiguracaoDashboardService
    {
        Task<ConfiguracaoDashboardDTO> Obter(string usuarioLogin);
        Task<ConfiguracaoDashboardDTO> Salvar(string usuarioLogin, ConfiguracaoDashboardDTO dto);
    }
}
