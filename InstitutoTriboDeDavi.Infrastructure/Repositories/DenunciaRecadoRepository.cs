using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class DenunciaRecadoRepository
        : BaseRepository<DenunciaRecado>, IDenunciaRecadoRepository
    {
        private readonly TriboDeDaviContext _context;

        public DenunciaRecadoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DenunciaRecado>> ListarPendentesAsync()
        {
            return await _context.DenunciasRecado
                .Where(d => !d.Resolvida)
                .AsNoTracking()
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();
        }

        public async Task RemoverPorRecadoAsync(long recadoId)
        {
            var denuncias = await _context.DenunciasRecado
                .Where(d => d.RecadoId == recadoId)
                .ToListAsync();
            if (denuncias.Count == 0) return;
            _context.DenunciasRecado.RemoveRange(denuncias);
            await _context.SaveChangesAsync();
        }
    }
}
