using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IDenunciaRecadoRepository : IBaseRepository<DenunciaRecado>
    {
        // Fila de denúncias em aberto (não resolvidas), mais recentes primeiro.
        Task<List<DenunciaRecado>> ListarPendentesAsync();
        // Remove todas as denúncias de um recado (usado ao excluir o recado).
        Task RemoverPorRecadoAsync(long recadoId);
    }
}
