using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IDoacaoRepository
    {
        Task<List<Doador>> ListarDoadoresAsync();
        Task<Doador> ObterDoadorAsync(long id);
        Task<Doador> SalvarDoadorAsync(Doador doador);
        // Só remove doador sem doações; caso contrário perderíamos o vínculo.
        Task<bool> ExcluirDoadorAsync(long id);

        Task<List<Doacao>> ListarDoacoesAsync(int? ano, long? doadorId);
        Task<Doacao> ObterDoacaoAsync(long id);
        Task<Doacao> SalvarDoacaoAsync(Doacao doacao);
        Task ExcluirDoacaoAsync(long id);
    }
}
