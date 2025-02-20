using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces
{
    public interface IAulaRepository : IBaseRepository<Aula>
    {
        Task<List<Aula>> ObterTodosAsync();
        Task<List<Aula>> ObterPorPoloTurmaAsync(long poloId, IEnumerable<int> turmas);
    }
}
