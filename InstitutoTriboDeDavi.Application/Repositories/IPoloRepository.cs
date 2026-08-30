using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IPoloRepository : IBaseRepository<Polo>
    {
        Task<Polo> GetByNome(string nome);
        Task<List<Polo>> SearchByNome(string nome);
        Task<List<Polo>> ObterTodosAsync();
        // Atualiza os dados do polo e substitui os horários por turma.
        Task<Polo> AtualizarComHorariosAsync(Polo polo);
    }
}
