using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IEmprestimoBemRepository : IBaseRepository<EmprestimoBem>
    {
        // Quantas alocações em aberto (sem devolução) cada bem tem, para calcular
        // a disponibilidade (Quantidade - abertas). Chave = BemPatrimonialId.
        Task<Dictionary<long, int>> ContarAbertosPorBemAsync();
        // Histórico completo de um bem, do mais recente para o mais antigo.
        Task<List<EmprestimoBem>> ListarPorBemAsync(long bemId);
        // Alocações de um aluno (abertas + histórico), mais recentes primeiro.
        Task<List<EmprestimoBem>> ListarPorAlunoAsync(long alunoId);
        // Alocações de um polo (abertas + histórico), mais recentes primeiro.
        Task<List<EmprestimoBem>> ListarPorPoloAsync(long poloId);
    }
}
