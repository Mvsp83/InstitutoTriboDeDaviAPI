using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class PushSubscriptionRepository : BaseRepository<PushSubscription>, IPushSubscriptionRepository
    {
        private readonly TriboDeDaviContext _context;

        public PushSubscriptionRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PushSubscription>> ObterPorUsuarioAsync(string usuarioLogin)
        {
            return await _context.PushSubscriptions
                .Where(x => x.UsuarioLogin == usuarioLogin)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PushSubscription> ObterPorEndpointAsync(string endpoint)
        {
            return await _context.PushSubscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Endpoint == endpoint);
        }

        public async Task RemoverPorEndpointAsync(string endpoint)
        {
            var existentes = await _context.PushSubscriptions
                .Where(x => x.Endpoint == endpoint)
                .ToListAsync();

            if (existentes.Count > 0)
            {
                _context.PushSubscriptions.RemoveRange(existentes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
