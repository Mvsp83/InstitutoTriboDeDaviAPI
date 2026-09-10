using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class MetricaRepository : IMetricaRepository
    {
        private readonly TriboDeDaviContext _context;

        public MetricaRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task IncrementarAsync(DateTime data, string chave, long delta)
        {
            var dia = data.Date;

            var existente = await _context.MetricasDiarias
                .FirstOrDefaultAsync(m => m.Data == dia && m.Chave == chave);

            if (existente == null)
            {
                _context.MetricasDiarias.Add(new MetricaDiaria
                {
                    Data = dia,
                    Chave = chave,
                    Valor = delta,
                });
            }
            else
            {
                existente.Valor += delta;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Corrida rara: outra requisição criou a mesma (dia, chave) ao
                // mesmo tempo (índice único). Reincrementa sobre a linha atual.
                _context.ChangeTracker.Clear();
                var atual = await _context.MetricasDiarias
                    .FirstOrDefaultAsync(m => m.Data == dia && m.Chave == chave);
                if (atual != null)
                {
                    atual.Valor += delta;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<MetricaDiaria>> ObterDesdeAsync(DateTime desde)
        {
            var dia = desde.Date;
            return await _context.MetricasDiarias
                .AsNoTracking()
                .Where(m => m.Data >= dia)
                .ToListAsync();
        }
    }
}
