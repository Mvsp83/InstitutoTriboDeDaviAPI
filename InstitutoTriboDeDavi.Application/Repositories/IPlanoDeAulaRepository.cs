using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IPlanoDeAulaRepository : IBaseRepository<PlanoDeAula>
    {
        Task<List<PlanoDeAula>> ObterTodosAsync();
        Task<List<PlanoDeAula>> ObterPorPoloTurmaAsync(long poloId, IEnumerable<int> turmas);
    }
}
