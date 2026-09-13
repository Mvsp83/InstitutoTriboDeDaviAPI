using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IEmprestimoBemRepository : IBaseRepository<EmprestimoBem>
    {
        // Empréstimo em aberto (sem devolução) de um bem, se houver.
        Task<EmprestimoBem> ObterAbertoPorBemAsync(long bemId);
        // Histórico completo de um bem, do mais recente para o mais antigo.
        Task<List<EmprestimoBem>> ListarPorBemAsync(long bemId);
    }
}
