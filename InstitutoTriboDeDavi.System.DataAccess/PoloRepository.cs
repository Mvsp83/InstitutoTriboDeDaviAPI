using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
{
    public class PoloRepository : BaseRepository<Polo>, IPoloRepository
    {
        private readonly TriboDeDaviContext _context;
        public PoloRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Polo> GetByNome(string nome)
        {
            var polo = await _context.Polos
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return polo.FirstOrDefault();
        }

        public async Task<List<Polo>> SearchByNome(string nome)
        {
            var allPolos = await _context.Polos
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allPolos;
        }
    }
}
