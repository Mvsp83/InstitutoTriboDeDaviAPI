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

        public async Task<Dictionary<long, int>> ContarAbertosPorBemAsync()
        {
            return await _context.Set<EmprestimoBem>()
                .AsNoTracking()
                .Where(e => e.DataDevolucao == null)
                .GroupBy(e => e.BemPatrimonialId)
                .Select(g => new { BemId = g.Key, Total = g.Count() })
                .ToDictionaryAsync(x => x.BemId, x => x.Total);
        }

        public async Task<List<EmprestimoBem>> ListarPorBemAsync(long bemId)
        {
            return await _context.Set<EmprestimoBem>()
                .AsNoTracking()
                .Where(e => e.BemPatrimonialId == bemId)
                .OrderByDescending(e => e.DataEmprestimo)
                .ToListAsync();
        }

        public async Task<List<EmprestimoBem>> ListarPorAlunoAsync(long alunoId)
        {
            return await _context.Set<EmprestimoBem>()
                .AsNoTracking()
                .Where(e => e.AlunoId == alunoId)
                .OrderByDescending(e => e.DataEmprestimo)
                .ToListAsync();
        }

        public async Task<List<EmprestimoBem>> ListarPorPoloAsync(long poloId)
        {
            return await _context.Set<EmprestimoBem>()
                .AsNoTracking()
                .Where(e => e.PoloId == poloId)
                .OrderByDescending(e => e.DataEmprestimo)
                .ToListAsync();
        }
    }
}
