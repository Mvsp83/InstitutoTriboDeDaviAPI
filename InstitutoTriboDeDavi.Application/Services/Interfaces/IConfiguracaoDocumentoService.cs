using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IConfiguracaoDocumentoService
    {
        Task<ConfiguracaoDocumentoDTO> Obter();
        Task<ConfiguracaoDocumentoDTO> Salvar(ConfiguracaoDocumentoDTO dto);
    }
}
