using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class MensagemContatoRepository
        : BaseRepository<MensagemContato>, IMensagemContatoRepository
    {
        private readonly TriboDeDaviContext _context;

        public MensagemContatoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<MensagemContato>> ObterTodasAsync()
        {
            return await _context.Set<MensagemContato>()
                .AsNoTracking()
                .OrderByDescending(m => m.DataCriacao)
                .ToListAsync();
        }

        public async Task<int> ContarNaoLidasAsync()
        {
            return await _context.Set<MensagemContato>()
                .CountAsync(m => !m.Lida);
        }
    }
}
