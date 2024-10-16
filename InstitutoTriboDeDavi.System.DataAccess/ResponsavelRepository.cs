using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
{
    public class ResponsavelRepository : BaseRepository<Responsavel>, IResponsavelRepository
    {
        private readonly TriboDeDaviContext _context;
        public ResponsavelRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Responsavel> GetByNome(string nome)
        {
            var responsavel = await _context.Responsaveis
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return responsavel.FirstOrDefault();
        }

        public async Task<List<Responsavel>> SearchByNome(string nome)
        {
            var allResponsaveis = await _context.Responsaveis
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allResponsaveis;
        }
    }
}
