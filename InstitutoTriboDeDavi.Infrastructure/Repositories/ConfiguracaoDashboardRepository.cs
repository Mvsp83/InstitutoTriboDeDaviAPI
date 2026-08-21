using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    // Uma linha por usuário em CONFIGURACAO_DASHBOARD (upsert por UsuarioLogin).
    public class ConfiguracaoDashboardRepository : IConfiguracaoDashboardRepository
    {
        private readonly TriboDeDaviContext _context;

        public ConfiguracaoDashboardRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracaoDashboard> ObterPorUsuarioAsync(string usuarioLogin)
        {
            return await _context.ConfiguracoesDashboard
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioLogin == usuarioLogin);
        }

        public async Task<ConfiguracaoDashboard> SalvarAsync(ConfiguracaoDashboard configuracao)
        {
            var existente = await _context.ConfiguracoesDashboard
                .FirstOrDefaultAsync(x => x.UsuarioLogin == configuracao.UsuarioLogin);

            if (existente == null)
            {
                _context.ConfiguracoesDashboard.Add(configuracao);
            }
            else
            {
                existente.Layout = configuracao.Layout;
            }

            await _context.SaveChangesAsync();
            return existente ?? configuracao;
        }
    }
}
