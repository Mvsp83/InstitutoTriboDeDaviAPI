using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class MensalidadesRepository : IMensalidadesRepository
    {
        private readonly TriboDeDaviContext _context;

        public MensalidadesRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        // ── Planos ────────────────────────────────────────────────────────
        public async Task<List<PlanoMensalidade>> ListarPlanosAsync()
        {
            return await _context.PlanosMensalidade
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<PlanoMensalidade> ObterPlanoAsync(long id)
        {
            return await _context.PlanosMensalidade
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PlanoMensalidade> SalvarPlanoAsync(PlanoMensalidade plano)
        {
            if (plano.Id > 0)
            {
                var existente = await _context.PlanosMensalidade.FirstOrDefaultAsync(p => p.Id == plano.Id);
                if (existente == null) return null;

                existente.Nome = plano.Nome;
                existente.Valor = plano.Valor;
                existente.OpcoesVencimento = plano.OpcoesVencimento;
                existente.Ativo = plano.Ativo;
                existente.Descricao = plano.Descricao;

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.PlanosMensalidade.Add(plano);
            await _context.SaveChangesAsync();
            return plano;
        }

        public async Task ExcluirPlanoAsync(long id)
        {
            var plano = await _context.PlanosMensalidade.FirstOrDefaultAsync(p => p.Id == id);
            if (plano != null)
            {
                _context.PlanosMensalidade.Remove(plano);
                await _context.SaveChangesAsync();
            }
        }

        // ── Matrículas ────────────────────────────────────────────────────
        public async Task<List<MatriculaFinanceira>> ListarMatriculasAsync()
        {
            return await _context.MatriculasFinanceiras
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MatriculaFinanceira>> ListarMatriculasAtivasAsync()
        {
            return await _context.MatriculasFinanceiras
                .AsNoTracking()
                .Where(m => m.Status == "ativo")
                .ToListAsync();
        }

        public async Task<MatriculaFinanceira> SalvarMatriculaAsync(MatriculaFinanceira matricula)
        {
            if (matricula.Id > 0)
            {
                var existente = await _context.MatriculasFinanceiras.FirstOrDefaultAsync(m => m.Id == matricula.Id);
                if (existente == null) return null;

                existente.AlunoId = matricula.AlunoId;
                existente.PlanoId = matricula.PlanoId;
                existente.DiaVencimento = matricula.DiaVencimento;
                existente.Inicio = matricula.Inicio;
                existente.Status = matricula.Status;
                existente.DescontoTipo = matricula.DescontoTipo;
                existente.DescontoValor = matricula.DescontoValor;
                existente.Observacao = matricula.Observacao;

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.MatriculasFinanceiras.Add(matricula);
            await _context.SaveChangesAsync();
            return matricula;
        }

        public async Task ExcluirMatriculaAsync(long id)
        {
            var matricula = await _context.MatriculasFinanceiras.FirstOrDefaultAsync(m => m.Id == id);
            if (matricula != null)
            {
                _context.MatriculasFinanceiras.Remove(matricula);
                await _context.SaveChangesAsync();
            }
        }

        // ── Cobranças ─────────────────────────────────────────────────────
        public async Task<List<Cobranca>> ListarCobrancasAsync(string competencia)
        {
            return await _context.Cobrancas
                .AsNoTracking()
                .Where(c => c.Competencia == competencia)
                .OrderBy(c => c.Vencimento)
                .ToListAsync();
        }

        public async Task<Cobranca> ObterCobrancaAsync(long id)
        {
            return await _context.Cobrancas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cobranca> SalvarCobrancaAsync(Cobranca cobranca)
        {
            if (cobranca.Id > 0)
            {
                var existente = await _context.Cobrancas.FirstOrDefaultAsync(c => c.Id == cobranca.Id);
                if (existente == null) return null;

                existente.AlunoId = cobranca.AlunoId;
                existente.PlanoId = cobranca.PlanoId;
                existente.Competencia = cobranca.Competencia;
                existente.Vencimento = cobranca.Vencimento;
                existente.Valor = cobranca.Valor;
                existente.Status = cobranca.Status;
                existente.PagamentoData = cobranca.PagamentoData;
                existente.PagamentoValor = cobranca.PagamentoValor;
                existente.PagamentoForma = cobranca.PagamentoForma;
                existente.ContaId = cobranca.ContaId;
                existente.Observacao = cobranca.Observacao;
                // MovimentacaoId não é alterado por aqui: só a baixa o define.

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.Cobrancas.Add(cobranca);
            await _context.SaveChangesAsync();
            return cobranca;
        }

        public async Task ExcluirCobrancaAsync(long id)
        {
            var cobranca = await _context.Cobrancas.FirstOrDefaultAsync(c => c.Id == id);
            if (cobranca != null)
            {
                _context.Cobrancas.Remove(cobranca);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<HashSet<long>> AlunosComCobrancaAsync(string competencia)
        {
            var ids = await _context.Cobrancas
                .AsNoTracking()
                .Where(c => c.Competencia == competencia)
                .Select(c => c.AlunoId)
                .ToListAsync();
            return ids.ToHashSet();
        }

        public async Task<int> AdicionarCobrancasAsync(List<Cobranca> cobrancas)
        {
            if (cobrancas == null || cobrancas.Count == 0) return 0;
            _context.Cobrancas.AddRange(cobrancas);
            await _context.SaveChangesAsync();
            return cobrancas.Count;
        }

        // Baixa: marca a cobrança como paga e cria a movimentação de receita no
        // livro-caixa (categoria "mensalidades"), tudo numa transação para não
        // deixar pagamento sem lançamento (ou vice-versa).
        public async Task<Cobranca> BaixarAsync(
            long id, DateTime pagamentoData, decimal pagamentoValor,
            string pagamentoForma, long contaId)
        {
            var cobranca = await _context.Cobrancas.FirstOrDefaultAsync(c => c.Id == id);
            if (cobranca == null) return null;

            var conta = await _context.ContasFinanceiras.FirstOrDefaultAsync(c => c.Id == contaId);
            if (conta == null)
                throw new DomainException("A conta informada não existe!");

            using var tx = await _context.Database.BeginTransactionAsync();

            var aluno = await _context.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == cobranca.AlunoId);
            var descricao = $"Mensalidade {cobranca.Competencia}"
                + (aluno != null ? $" — {aluno.Nome}" : string.Empty);

            var movimentacao = new MovimentacaoFinanceira
            {
                ContaId = contaId,
                Data = pagamentoData,
                Descricao = descricao.Length > 200 ? descricao.Substring(0, 200) : descricao,
                CategoriaId = "mensalidades",
                Tipo = "Credito",
                Valor = pagamentoValor,
                Conciliado = false,
                Documento = string.Empty,
                Observacoes = string.Empty,
                TransferenciaId = string.Empty,
            };

            _context.MovimentacoesFinanceiras.Add(movimentacao);
            await _context.SaveChangesAsync();

            cobranca.Status = "pago";
            cobranca.PagamentoData = pagamentoData;
            cobranca.PagamentoValor = pagamentoValor;
            cobranca.PagamentoForma = pagamentoForma;
            cobranca.ContaId = contaId;
            cobranca.MovimentacaoId = movimentacao.Id;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return cobranca;
        }
    }
}
