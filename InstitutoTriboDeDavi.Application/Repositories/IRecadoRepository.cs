using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IRecadoRepository : IBaseRepository<Recado>
    {
        // Recados vigentes do mural: ativos e ainda não expirados, recentes primeiro.
        Task<List<Recado>> ObterVigentesAsync();
        // Todos (inclui expirados/inativos) — para a tela de gestão da equipe.
        Task<List<Recado>> ObterTodosAsync();
    }
}
