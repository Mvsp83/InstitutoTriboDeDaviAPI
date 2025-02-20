using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess.Business
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
