using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
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

        public async Task<List<Polo>> ObterTodosAsync()
        {
            return await _context.Polos.ToListAsync();
        }

    }
}
