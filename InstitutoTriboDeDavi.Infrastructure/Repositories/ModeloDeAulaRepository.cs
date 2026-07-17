using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class ModeloDeAulaRepository : BaseRepository<ModeloDeAula>, IModeloDeAulaRepository
    {
        private readonly TriboDeDaviContext _context;

        public ModeloDeAulaRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ModeloDeAula> GetByIdAsync(long id)
        {
            return await _context.ModelosDeAula
                .AsNoTracking()
                .Include(m => m.Blocos.OrderBy(b => b.Ordem))
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<List<ModeloDeAula>> GetAllAsync()
        {
            return await _context.ModelosDeAula
                .AsNoTracking()
                .Include(m => m.Blocos.OrderBy(b => b.Ordem))
                .ToListAsync();
        }

        // Update com filhos: a coleção de blocos enviada substitui a existente
        public override async Task<ModeloDeAula> UpdateAsync(ModeloDeAula obj)
        {
            var existente = await _context.ModelosDeAula
                .Include(m => m.Blocos)
                .FirstOrDefaultAsync(m => m.Id == obj.Id);

            if (existente == null)
                return null;

            _context.Entry(existente).CurrentValues.SetValues(obj);

            _context.RemoveRange(existente.Blocos);
            existente.Blocos.Clear();

            foreach (var bloco in obj.Blocos.OrderBy(b => b.Ordem))
            {
                bloco.Id = 0;
                bloco.ModeloDeAulaId = existente.Id;
                existente.Blocos.Add(bloco);
            }

            await _context.SaveChangesAsync();

            return existente;
        }
    }
}
