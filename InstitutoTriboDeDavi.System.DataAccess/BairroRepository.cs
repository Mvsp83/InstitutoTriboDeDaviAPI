using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
{
    public class BairroRepository : BaseRepository<Bairro>, IBairroRepository
    {
        private readonly TriboDeDaviContext _context;
        public BairroRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Bairro> GetByNome(string nome)
        {
            var bairro = await _context.Bairros
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return bairro.FirstOrDefault();
        }

        public async Task<List<Bairro>> SearchByNome(string nome)
        {
            var allBairros = await _context.Bairros
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allBairros;
        }
    }
}
