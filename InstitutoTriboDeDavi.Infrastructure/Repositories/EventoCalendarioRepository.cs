using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class EventoCalendarioRepository : BaseRepository<EventoCalendario>, IEventoCalendarioRepository
    {
        private readonly TriboDeDaviContext _context;

        public EventoCalendarioRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<EventoCalendario>> ObterPorAnoAsync(int ano)
        {
            return await _context.EventosCalendario
                .Where(e => e.Ano == ano)
                .AsNoTracking()
                .OrderBy(e => e.Data)
                .ToListAsync();
        }

        public async Task<List<int>> ObterAnosAsync()
        {
            return await _context.EventosCalendario
                .Select(e => e.Ano)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task CriarVariosAsync(IEnumerable<EventoCalendario> eventos)
        {
            _context.EventosCalendario.AddRange(eventos);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EventoCalendario>> ObterPendentesNotificacaoAsync()
        {
            return await _context.EventosCalendario
                .Where(e => e.Notificar && !e.NotificacaoEnviada)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task MarcarNotificadaAsync(long id)
        {
            var evento = await _context.EventosCalendario.FindAsync(id);
            if (evento != null)
            {
                evento.NotificacaoEnviada = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
