using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class SincronizacaoHistoricoRepository : BaseRepository<SincronizacaoHistorico>, ISincronizacaoHistoricoRepository
    {
        private readonly TriboDeDaviContext _context;

        public SincronizacaoHistoricoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SincronizacaoHistorico>> ObterUltimasAsync(int quantidade = 50)
        {
            return await _context.SincronizacaoHistoricos
                .OrderByDescending(s => s.DataExecucao)
                .Take(quantidade)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SincronizacaoHistorico>> ObterPorPoloAsync(long poloId, int quantidade = 20)
        {
            return await _context.SincronizacaoHistoricos
                .Where(s => s.PoloId == poloId)
                .OrderByDescending(s => s.DataExecucao)
                .Take(quantidade)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SincronizacaoHistorico> ObterUltimaExecucaoAsync()
        {
            return await _context.SincronizacaoHistoricos
                .OrderByDescending(s => s.DataExecucao)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }

}
