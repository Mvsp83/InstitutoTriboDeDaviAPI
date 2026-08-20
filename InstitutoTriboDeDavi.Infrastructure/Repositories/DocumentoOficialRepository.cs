using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class DocumentoOficialRepository
        : BaseRepository<DocumentoOficial>, IDocumentoOficialRepository
    {
        private readonly TriboDeDaviContext _context;

        public DocumentoOficialRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DocumentoOficial>> ObterPorAnoAsync(int ano)
        {
            // Ano do rascunho ainda pode variar; filtramos pelo ano da data do
            // documento (rascunhos) ou pelo ano oficial (aprovados).
            return await _context.DocumentosOficiais
                .Where(d => d.Ano == ano || d.DataDocumento.Year == ano)
                .AsNoTracking()
                .OrderByDescending(d => d.DataDocumento)
                .ToListAsync();
        }

        public async Task<List<int>> ObterAnosAsync()
        {
            return await _context.DocumentosOficiais
                .Select(d => d.DataDocumento.Year)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<DocumentoOficial> AprovarAsync(long id)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

            var doc = await _context.DocumentosOficiais.FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                throw new DomainException("Documento não encontrado.");
            if (doc.Status == 1)
                throw new DomainException("Este documento já foi aprovado.");

            var ano = doc.DataDocumento.Year;

            // Próximo número da sequência daquele tipo, naquele ano (só aprovados).
            var maior = await _context.DocumentosOficiais
                .Where(d => d.Tipo == doc.Tipo && d.Status == 1 && d.Ano == ano)
                .Select(d => (int?)d.Numero)
                .MaxAsync() ?? 0;

            doc.Ano = ano;
            doc.Numero = maior + 1;
            doc.NumeroFormatado = $"{ano}/{doc.Numero:D4}";
            doc.Status = 1;
            doc.DataAprovacao = DateTime.Now;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return doc;
        }
    }
}
