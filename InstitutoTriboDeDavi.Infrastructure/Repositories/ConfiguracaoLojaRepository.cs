using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    // Singleton: existe no máximo uma linha em CONFIGURACAO_LOJA.
    public class ConfiguracaoLojaRepository : IConfiguracaoLojaRepository
    {
        private readonly TriboDeDaviContext _context;

        public ConfiguracaoLojaRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracaoLoja> ObterAsync()
        {
            return await _context.ConfiguracoesLoja
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ConfiguracaoLoja> SalvarAsync(ConfiguracaoLoja configuracao)
        {
            var existente = await _context.ConfiguracoesLoja.FirstOrDefaultAsync();

            if (existente == null)
            {
                _context.ConfiguracoesLoja.Add(configuracao);
            }
            else
            {
                existente.CompraWhatsappHabilitada = configuracao.CompraWhatsappHabilitada;
                existente.WhatsappNumero = configuracao.WhatsappNumero;
            }

            await _context.SaveChangesAsync();
            return existente ?? configuracao;
        }
    }
}
