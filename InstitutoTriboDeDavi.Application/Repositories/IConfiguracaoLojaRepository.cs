using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IConfiguracaoLojaRepository
    {
        Task<ConfiguracaoLoja> ObterAsync();
        Task<ConfiguracaoLoja> SalvarAsync(ConfiguracaoLoja configuracao);
    }
}
