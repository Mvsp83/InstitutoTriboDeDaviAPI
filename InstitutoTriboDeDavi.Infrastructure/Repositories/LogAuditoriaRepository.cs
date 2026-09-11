using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class LogAuditoriaRepository : ILogAuditoriaRepository
    {
        private readonly TriboDeDaviContext _context;

        public LogAuditoriaRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<LogAuditoria>> ListarAsync(string entidade, string usuario, int limite)
        {
            var query = _context.LogsAuditoria.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(entidade))
                query = query.Where(l => l.Entidade == entidade);

            if (!string.IsNullOrWhiteSpace(usuario))
                query = query.Where(l => l.UsuarioLogin.Contains(usuario));

            return await query
                .OrderByDescending(l => l.Data)
                .Take(limite > 0 && limite <= 500 ? limite : 100)
                .ToListAsync();
        }

        public async Task<int> LimparAnterioresAsync(DateTime limite)
        {
            return await _context.LogsAuditoria
                .Where(l => l.Data < limite)
                .ExecuteDeleteAsync();
        }
    }
}
