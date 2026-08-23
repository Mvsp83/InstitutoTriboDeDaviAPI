using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class DoacaoRepository : IDoacaoRepository
    {
        private readonly TriboDeDaviContext _context;

        public DoacaoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        // ── Doadores ──────────────────────────────────────────────────────
        public async Task<List<Doador>> ListarDoadoresAsync()
        {
            return await _context.Doadores.AsNoTracking().OrderBy(d => d.Nome).ToListAsync();
        }

        public async Task<Doador> ObterDoadorAsync(long id)
        {
            return await _context.Doadores.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Doador> SalvarDoadorAsync(Doador doador)
        {
            if (doador.Id > 0)
            {
                var existente = await _context.Doadores.FirstOrDefaultAsync(d => d.Id == doador.Id);
                if (existente == null) return null;

                existente.TipoPessoa = doador.TipoPessoa;
                existente.Nome = doador.Nome;
                existente.Documento = doador.Documento;
                existente.Email = doador.Email;
                existente.Telefone = doador.Telefone;
                existente.Endereco = doador.Endereco;
                existente.Cidade = doador.Cidade;
                existente.Observacoes = doador.Observacoes;
                existente.Ativo = doador.Ativo;

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.Doadores.Add(doador);
            await _context.SaveChangesAsync();
            return doador;
        }

        public async Task<bool> ExcluirDoadorAsync(long id)
        {
            // Doador com histórico não é removido: apagá-lo deixaria as doações
            // órfãs e sumiria com a memória de quem apoiou o projeto.
            if (await _context.Doacoes.AnyAsync(d => d.DoadorId == id)) return false;

            var doador = await _context.Doadores.FirstOrDefaultAsync(d => d.Id == id);
            if (doador == null) return true;

            _context.Doadores.Remove(doador);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── Doações ───────────────────────────────────────────────────────
        public async Task<List<Doacao>> ListarDoacoesAsync(int? ano, long? doadorId)
        {
            var query = _context.Doacoes.AsNoTracking().AsQueryable();

            if (ano.HasValue) query = query.Where(d => d.Data.Year == ano.Value);
            if (doadorId.HasValue) query = query.Where(d => d.DoadorId == doadorId.Value);

            return await query.OrderByDescending(d => d.Data).ToListAsync();
        }

        public async Task<Doacao> ObterDoacaoAsync(long id)
        {
            return await _context.Doacoes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task VincularReciboAsync(long doacaoId, long reciboDocumentoId, string reciboNumero)
        {
            var doacao = await _context.Doacoes.FirstOrDefaultAsync(d => d.Id == doacaoId);
            if (doacao == null) return;

            doacao.ReciboDocumentoId = reciboDocumentoId;
            doacao.ReciboNumero = reciboNumero ?? string.Empty;
            await _context.SaveChangesAsync();
        }

        public async Task<Doacao> SalvarDoacaoAsync(Doacao doacao)
        {
            if (doacao.Id > 0)
            {
                var existente = await _context.Doacoes.FirstOrDefaultAsync(d => d.Id == doacao.Id);
                if (existente == null) return null;

                existente.DoadorId = doacao.DoadorId;
                existente.Valor = doacao.Valor;
                existente.Data = doacao.Data;
                existente.Forma = doacao.Forma;
                existente.Finalidade = doacao.Finalidade;
                existente.Observacoes = doacao.Observacoes;
                // Recibo emitido não é alterado por edição da doação.

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.Doacoes.Add(doacao);
            await _context.SaveChangesAsync();
            return doacao;
        }

        public async Task ExcluirDoacaoAsync(long id)
        {
            var doacao = await _context.Doacoes.FirstOrDefaultAsync(d => d.Id == id);
            if (doacao == null) return;

            _context.Doacoes.Remove(doacao);
            await _context.SaveChangesAsync();
        }
    }
}
