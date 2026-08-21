using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class GraduacaoRepository : IGraduacaoRepository
    {
        private readonly TriboDeDaviContext _context;

        public GraduacaoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<Graduacao>> ListarAsync(int? ano, long? poloId)
        {
            var query = _context.Graduacoes.AsNoTracking().AsQueryable();

            if (ano.HasValue) query = query.Where(g => g.Data.Year == ano.Value);
            if (poloId.HasValue) query = query.Where(g => g.PoloId == poloId.Value);

            return await query.OrderByDescending(g => g.Data).ToListAsync();
        }

        public async Task<List<Graduacao>> ListarPorAlunoAsync(long alunoId)
        {
            return await _context.Graduacoes
                .AsNoTracking()
                .Where(g => g.AlunoId == alunoId)
                .OrderBy(g => g.Data)
                .ToListAsync();
        }

        public async Task<Graduacao> ObterAsync(long id)
        {
            return await _context.Graduacoes
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<int> RegistrarAsync(List<(Graduacao graduacao, Aluno aluno)> itens)
        {
            if (itens.Count == 0) return 0;

            using var tx = await _context.Database.BeginTransactionAsync();

            foreach (var (graduacao, aluno) in itens)
            {
                _context.Graduacoes.Add(graduacao);
                // A faixa atual do aluno acompanha o registro.
                aluno.Faixa = (Domain.Enums.Faixa)graduacao.FaixaNova;
                _context.Alunos.Update(aluno);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return itens.Count;
        }

        public async Task ExcluirAsync(Graduacao graduacao, Aluno aluno)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var existente = await _context.Graduacoes.FirstOrDefaultAsync(g => g.Id == graduacao.Id);
            if (existente != null) _context.Graduacoes.Remove(existente);

            if (aluno != null)
            {
                // Desfazer a graduação devolve a faixa que o aluno tinha antes.
                aluno.Faixa = (Domain.Enums.Faixa)graduacao.FaixaAnterior;
                _context.Alunos.Update(aluno);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
    }
}
