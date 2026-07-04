using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class AulaRepository : BaseRepository<Aula>, IAulaRepository
    {
        private readonly TriboDeDaviContext _context;

        public AulaRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Aula>> ObterTodosAsync()
        {
            return await _context.Aulas.ToListAsync();
        }

        public async Task<List<Aula>> ObterPorPoloTurmaAsync(long poloId, IEnumerable<int> turmas)
        {
            return await _context.Aulas
                                 .Where(a => a.PoloId == poloId && turmas.Contains(a.Turma))
                                 .ToListAsync();
        }
    }
}
