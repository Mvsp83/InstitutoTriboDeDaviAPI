using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class EmprestimoBemRepository
        : BaseRepository<EmprestimoBem>, IEmprestimoBemRepository
    {
        private readonly TriboDeDaviContext _context;

        public EmprestimoBemRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EmprestimoBem> ObterAbertoPorBemAsync(long bemId)
        {
            return await _context.Set<EmprestimoBem>()
                .AsNoTracking()
                .Where(e => e.BemPatrimonialId == bemId && e.DataDevolucao == null)
                .OrderByDescending(e => e.DataEmprestimo)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmprestimoBem>> ListarPorBemAsync(long bemId)
        {
            return await _context.Set<EmprestimoBem>()
                .AsNoTracking()
                .Where(e => e.BemPatrimonialId == bemId)
                .OrderByDescending(e => e.DataEmprestimo)
                .ToListAsync();
        }
    }
}
