using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IConfiguracaoDocumentoRepository
    {
        Task<ConfiguracaoDocumento> ObterAsync();
        Task<ConfiguracaoDocumento> SalvarAsync(ConfiguracaoDocumento configuracao);
    }
}
