using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class DocumentoArquivoRepository : IDocumentoArquivoRepository
    {
        private readonly TriboDeDaviContext _context;

        public DocumentoArquivoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<DocumentoArquivo>> ListarPorCategoriaAsync(int categoria)
        {
            // Projeção sem Conteudo: nunca traz o binário na listagem.
            return await _context.DocumentosArquivo
                .AsNoTracking()
                .Where(d => d.Categoria == categoria)
                .OrderByDescending(d => d.DataCriacao)
                .Select(d => new DocumentoArquivo
                {
                    Id = d.Id,
                    Categoria = d.Categoria,
                    Nome = d.Nome,
                    ContentType = d.ContentType,
                    TamanhoBytes = d.TamanhoBytes,
                    DataCriacao = d.DataCriacao,
                    Conteudo = null,
                })
                .ToListAsync();
        }

        public async Task<DocumentoArquivo> ObterAsync(long id)
        {
            return await _context.DocumentosArquivo
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DocumentoArquivo> CriarAsync(DocumentoArquivo documento)
        {
            _context.DocumentosArquivo.Add(documento);
            await _context.SaveChangesAsync();
            return documento;
        }

        public async Task<bool> ExcluirAsync(long id)
        {
            var doc = await _context.DocumentosArquivo.FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null) return false;

            _context.DocumentosArquivo.Remove(doc);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
