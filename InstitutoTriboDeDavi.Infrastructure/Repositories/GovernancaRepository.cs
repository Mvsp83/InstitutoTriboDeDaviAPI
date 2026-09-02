using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class GovernancaRepository : IGovernancaRepository
    {
        private readonly TriboDeDaviContext _context;

        public GovernancaRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<MembroGovernanca>> ListarPorAnoAsync(int ano)
        {
            return await _context.MembrosGovernanca
                .AsNoTracking()
                .Where(m => m.Ano == ano)
                .OrderBy(m => m.Orgao)
                .ThenBy(m => m.Ordem)
                .ToListAsync();
        }

        public async Task<List<int>> ListarAnosAsync()
        {
            return await _context.MembrosGovernanca
                .AsNoTracking()
                .Select(m => m.Ano)
                .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();
        }

        public async Task SubstituirAnoAsync(int ano, List<MembroGovernanca> membros)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var antigos = await _context.MembrosGovernanca
                .Where(m => m.Ano == ano)
                .ToListAsync();
            _context.MembrosGovernanca.RemoveRange(antigos);

            if (membros.Count > 0)
                await _context.MembrosGovernanca.AddRangeAsync(membros);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
    }
}
