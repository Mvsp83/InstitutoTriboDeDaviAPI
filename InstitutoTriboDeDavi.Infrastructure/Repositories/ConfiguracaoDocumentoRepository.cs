using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    // Singleton: existe no máximo uma linha em CONFIGURACAO_DOCUMENTO.
    public class ConfiguracaoDocumentoRepository : IConfiguracaoDocumentoRepository
    {
        private readonly TriboDeDaviContext _context;

        public ConfiguracaoDocumentoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracaoDocumento> ObterAsync()
        {
            return await _context.ConfiguracoesDocumento
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ConfiguracaoDocumento> SalvarAsync(ConfiguracaoDocumento configuracao)
        {
            var existente = await _context.ConfiguracoesDocumento.FirstOrDefaultAsync();

            if (existente == null)
            {
                _context.ConfiguracoesDocumento.Add(configuracao);
            }
            else
            {
                existente.TituloCabecalho = configuracao.TituloCabecalho;
                existente.LinhaExtra = configuracao.LinhaExtra;
                existente.TextoRodape = configuracao.TextoRodape;
                existente.MostrarLogo = configuracao.MostrarLogo;
                existente.MostrarDataGeracao = configuracao.MostrarDataGeracao;
            }

            await _context.SaveChangesAsync();
            return existente ?? configuracao;
        }
    }
}
