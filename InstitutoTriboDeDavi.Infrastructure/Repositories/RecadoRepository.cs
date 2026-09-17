using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class RecadoRepository : BaseRepository<Recado>, IRecadoRepository
    {
        private readonly TriboDeDaviContext _context;

        public RecadoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Recado>> ObterVigentesAsync()
        {
            var agora = DateTime.Now;
            return await _context.Recados
                .Where(r => r.Ativo && r.ExpiraEm >= agora)
                .AsNoTracking()
                .OrderByDescending(r => r.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<Recado>> ObterTodosAsync()
        {
            return await _context.Recados
                .AsNoTracking()
                .OrderByDescending(r => r.DataCriacao)
                .ToListAsync();
        }
    }
}
