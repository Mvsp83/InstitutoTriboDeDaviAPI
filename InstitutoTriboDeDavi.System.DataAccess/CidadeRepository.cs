using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
{
    public class CidadeRepository : BaseRepository<Cidade>, ICidadeRepository
    {
        private readonly TriboDeDaviContext _context;

        public CidadeRepository(TriboDeDaviContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Cidade> GetByNome(string nome)
        {
            var cidade = await _context.Cidades
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return cidade.FirstOrDefault();
        }

        public async Task<List<Cidade>> SearchByNome(string nome)
        {
            var allCidades = await _context.Cidades
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allCidades;
        }
    }
}
