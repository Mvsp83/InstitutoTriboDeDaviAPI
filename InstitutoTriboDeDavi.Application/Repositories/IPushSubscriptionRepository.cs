using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IPushSubscriptionRepository : IBaseRepository<PushSubscription>
    {
        // Todas as inscrições (dispositivos) de um usuário.
        Task<List<PushSubscription>> ObterPorUsuarioAsync(string usuarioLogin);

        // Inscrição por endpoint (único por dispositivo), ou null.
        Task<PushSubscription> ObterPorEndpointAsync(string endpoint);

        // Remove a inscrição de um endpoint, se existir (idempotente).
        Task RemoverPorEndpointAsync(string endpoint);
    }
}
