using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IAlunoRepository : IBaseRepository<Aluno>
    {
        Task<Aluno> GetByNome(string nome);
        Task<Aluno> GetByCpf(string cpf);
        Task<List<Aluno>> SearchByNome(string nome);
        Task<int> GetTotalAlunosAsync();
        Task<List<Aluno>> ObterTodosAsync();
        Task<List<Aluno>> ObterPorPoloTurmaAsync(long poloId, List<int> turmas);
        Task<List<Aluno>> ObterPendentesPorPoloAsync(long poloId);
        Task<List<Aluno>> ObterTodosPendentesAsync();
    }
}
