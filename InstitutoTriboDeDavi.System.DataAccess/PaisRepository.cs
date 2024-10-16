using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
{
    public class PaisRepository : BaseRepository<Pais>, IPaisRepository
    {
        private readonly TriboDeDaviContext _context;
        public PaisRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pais> GetByNome(string nome)
        {
            var pais = await _context.Paises
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return pais.FirstOrDefault();
        }

        public async Task<List<Pais>> SearchByNome(string nome)
        {
            var allPaises = await _context.Paises
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allPaises;
        }
    }
}
