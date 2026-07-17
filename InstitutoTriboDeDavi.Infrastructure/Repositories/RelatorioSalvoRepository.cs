using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class RelatorioSalvoRepository : BaseRepository<RelatorioSalvo>, IRelatorioSalvoRepository
    {
        private readonly TriboDeDaviContext _context;

        public RelatorioSalvoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RelatorioSalvo>> GetByUsuarioAsync(string usuarioLogin)
        {
            return await _context.RelatoriosSalvos
                .AsNoTracking()
                .Where(r => r.UsuarioLogin == usuarioLogin)
                .OrderBy(r => r.Nome)
                .ToListAsync();
        }
    }
}
