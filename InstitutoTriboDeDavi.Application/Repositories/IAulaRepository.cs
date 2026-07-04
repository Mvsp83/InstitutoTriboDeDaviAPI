using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IAulaRepository : IBaseRepository<Aula>
    {
        Task<List<Aula>> ObterTodosAsync();
        Task<List<Aula>> ObterPorPoloTurmaAsync(long poloId, IEnumerable<int> turmas);
    }
}
