using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
{
    public class EstadoRepository : BaseRepository<Estado>, IEstadoRepository
    {
        private readonly TriboDeDaviContext _context;
        public EstadoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Estado> GetByNome(string nome)
        {
            var estado = await _context.Estados
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return estado.FirstOrDefault();
        }

        public async Task<List<Estado>> SearchByNome(string nome)
        {
            var allEstados = await _context.Estados
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allEstados;
        }
    }
}
