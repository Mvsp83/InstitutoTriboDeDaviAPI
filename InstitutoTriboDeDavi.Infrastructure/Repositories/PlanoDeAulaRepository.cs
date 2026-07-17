using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class PlanoDeAulaRepository : BaseRepository<PlanoDeAula>, IPlanoDeAulaRepository
    {
        private readonly TriboDeDaviContext _context;

        public PlanoDeAulaRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<PlanoDeAula> GetByIdAsync(long id)
        {
            return await _context.PlanosDeAula
                .AsNoTracking()
                .Include(p => p.Blocos.OrderBy(b => b.Ordem))
                .ThenInclude(b => b.Atividades)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<List<PlanoDeAula>> GetAllAsync()
        {
            return await _context.PlanosDeAula
                .AsNoTracking()
                .Include(p => p.Blocos.OrderBy(b => b.Ordem))
                .ThenInclude(b => b.Atividades)
                .ToListAsync();
        }

        // Update com filhos: a coleção de blocos enviada substitui a existente
        public override async Task<PlanoDeAula> UpdateAsync(PlanoDeAula obj)
        {
            var existente = await _context.PlanosDeAula
                .Include(p => p.Blocos)
                .ThenInclude(b => b.Atividades)
                .FirstOrDefaultAsync(p => p.Id == obj.Id);

            if (existente == null)
                return null;

            _context.Entry(existente).CurrentValues.SetValues(obj);

            _context.RemoveRange(existente.Blocos.SelectMany(b => b.Atividades));
            _context.RemoveRange(existente.Blocos);
            existente.Blocos.Clear();

            foreach (var bloco in obj.Blocos.OrderBy(b => b.Ordem))
            {
                bloco.Id = 0;
                bloco.PlanoDeAulaId = existente.Id;

                foreach (var atividade in bloco.Atividades)
                {
                    atividade.Id = 0;
                    atividade.BlocoDoPlanoId = 0;
                }

                existente.Blocos.Add(bloco);
            }

            await _context.SaveChangesAsync();

            return existente;
        }

        public async Task<List<PlanoDeAula>> ObterTodosAsync()
        {
            return await GetAllAsync();
        }

        public async Task<List<PlanoDeAula>> ObterPorPoloTurmaAsync(long poloId, IEnumerable<int> turmas)
        {
            return await _context.PlanosDeAula
                .AsNoTracking()
                .Include(p => p.Blocos.OrderBy(b => b.Ordem))
                .ThenInclude(b => b.Atividades)
                .Where(p => p.PoloId == poloId && turmas.Contains(p.Turma))
                .ToListAsync();
        }
    }
}
