using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IConfiguracaoLojaService
    {
        Task<ConfiguracaoLojaDTO> Obter();
        Task<ConfiguracaoLojaDTO> Salvar(ConfiguracaoLojaDTO dto);
    }
}
