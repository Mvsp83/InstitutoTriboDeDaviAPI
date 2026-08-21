using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class FinanceiroRepository : IFinanceiroRepository
    {
        private readonly TriboDeDaviContext _context;

        public FinanceiroRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        // ── Contas ────────────────────────────────────────────────────────
        public async Task<List<ContaFinanceira>> ListarContasAsync()
        {
            return await _context.ContasFinanceiras
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<ContaFinanceira> ObterContaAsync(long id)
        {
            return await _context.ContasFinanceiras
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ContaFinanceira> SalvarContaAsync(ContaFinanceira conta)
        {
            if (conta.Id > 0)
            {
                var existente = await _context.ContasFinanceiras.FirstOrDefaultAsync(c => c.Id == conta.Id);
                if (existente == null) return null;

                existente.Tipo = conta.Tipo;
                existente.Nome = conta.Nome;
                existente.Banco = conta.Banco;
                existente.Agencia = conta.Agencia;
                existente.Numero = conta.Numero;
                existente.SaldoInicial = conta.SaldoInicial;
                existente.Ativa = conta.Ativa;
                existente.Observacoes = conta.Observacoes;

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.ContasFinanceiras.Add(conta);
            await _context.SaveChangesAsync();
            return conta;
        }

        public async Task ExcluirContaAsync(long id)
        {
            // Conta e lançamentos saem juntos para não deixar lançamento órfão.
            using var tx = await _context.Database.BeginTransactionAsync();

            var movs = await _context.MovimentacoesFinanceiras
                .Where(m => m.ContaId == id)
                .ToListAsync();
            if (movs.Count > 0) _context.MovimentacoesFinanceiras.RemoveRange(movs);

            var conta = await _context.ContasFinanceiras.FirstOrDefaultAsync(c => c.Id == id);
            if (conta != null) _context.ContasFinanceiras.Remove(conta);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }

        // ── Movimentações ─────────────────────────────────────────────────
        public async Task<List<MovimentacaoFinanceira>> ListarMovimentacoesAsync()
        {
            return await _context.MovimentacoesFinanceiras
                .AsNoTracking()
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }

        public async Task<MovimentacaoFinanceira> ObterMovimentacaoAsync(long id)
        {
            return await _context.MovimentacoesFinanceiras
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MovimentacaoFinanceira> SalvarMovimentacaoAsync(MovimentacaoFinanceira mov)
        {
            if (mov.Id > 0)
            {
                var existente = await _context.MovimentacoesFinanceiras.FirstOrDefaultAsync(m => m.Id == mov.Id);
                if (existente == null) return null;

                existente.ContaId = mov.ContaId;
                existente.Data = mov.Data;
                existente.Descricao = mov.Descricao;
                existente.CategoriaId = mov.CategoriaId;
                existente.Tipo = mov.Tipo;
                existente.Valor = mov.Valor;
                existente.Conciliado = mov.Conciliado;
                existente.Documento = mov.Documento;
                existente.Observacoes = mov.Observacoes;
                // TransferenciaId não é alterado: o par é criado/excluído junto.

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.MovimentacoesFinanceiras.Add(mov);
            await _context.SaveChangesAsync();
            return mov;
        }

        public async Task ExcluirMovimentacaoAsync(long id)
        {
            var alvo = await _context.MovimentacoesFinanceiras.FirstOrDefaultAsync(m => m.Id == id);
            if (alvo == null) return;

            // Transferência é um par: excluir um lado remove o outro.
            if (!string.IsNullOrEmpty(alvo.TransferenciaId))
            {
                var par = await _context.MovimentacoesFinanceiras
                    .Where(m => m.TransferenciaId == alvo.TransferenciaId)
                    .ToListAsync();
                _context.MovimentacoesFinanceiras.RemoveRange(par);
            }
            else
            {
                _context.MovimentacoesFinanceiras.Remove(alvo);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DefinirConciliacaoAsync(long id, bool conciliado)
        {
            var mov = await _context.MovimentacoesFinanceiras.FirstOrDefaultAsync(m => m.Id == id);
            if (mov == null) return;

            mov.Conciliado = conciliado;
            await _context.SaveChangesAsync();
        }

        public async Task RegistrarTransferenciaAsync(MovimentacaoFinanceira debito, MovimentacaoFinanceira credito)
        {
            // Os dois lados entram juntos: metade de uma transferência gravada
            // deixaria o saldo das contas errado.
            using var tx = await _context.Database.BeginTransactionAsync();

            _context.MovimentacoesFinanceiras.AddRange(debito, credito);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
        }

        // ── Importação (migração do localStorage) ─────────────────────────
        public async Task<bool> ExisteAlgumDadoAsync()
        {
            return await _context.ContasFinanceiras.AnyAsync()
                || await _context.MovimentacoesFinanceiras.AnyAsync();
        }

        public async Task<(int contas, int movimentacoes)> ImportarAsync(
            List<(ContaFinanceira conta, long idOrigem)> contas,
            List<(MovimentacaoFinanceira mov, long contaIdOrigem)> movimentacoes)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            // As contas ganham ids novos do banco; guardamos o de-para para
            // religar os lançamentos, que referenciam os ids antigos do navegador.
            var deParaConta = new Dictionary<long, long>();

            foreach (var (conta, idOrigem) in contas)
            {
                conta.Id = 0;
                _context.ContasFinanceiras.Add(conta);
                await _context.SaveChangesAsync();
                deParaConta[idOrigem] = conta.Id;
            }

            var importadas = 0;
            foreach (var (mov, contaIdOrigem) in movimentacoes)
            {
                if (!deParaConta.TryGetValue(contaIdOrigem, out var contaIdNovo))
                    continue; // lançamento sem conta correspondente é descartado

                mov.Id = 0;
                mov.ContaId = contaIdNovo;
                _context.MovimentacoesFinanceiras.Add(mov);
                importadas++;
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return (deParaConta.Count, importadas);
        }
    }
}
