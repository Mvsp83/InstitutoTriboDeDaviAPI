using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class InscricaoRepository : IInscricaoRepository
    {
        private readonly TriboDeDaviContext _context;

        public InscricaoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<Inscricao> CriarAsync(Inscricao inscricao)
        {
            _context.Inscricoes.Add(inscricao);
            await _context.SaveChangesAsync();
            return inscricao;
        }

        public async Task<Inscricao> ObterAsync(long id)
        {
            return await _context.Inscricoes
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Inscricao>> ListarAsync(int? status, int? ano, long? poloId)
        {
            var query = _context.Inscricoes.AsNoTracking().AsQueryable();

            if (status.HasValue) query = query.Where(i => i.Status == status.Value);
            if (ano.HasValue) query = query.Where(i => i.Ano == ano.Value);
            if (poloId.HasValue) query = query.Where(i => i.PoloId == poloId.Value);

            // Mais antigas primeiro: quem enviou antes é revisado antes.
            return await query.OrderBy(i => i.DataEnvio).ToListAsync();
        }

        public async Task<int> ContarPendentesAsync(long? poloId)
        {
            var query = _context.Inscricoes
                .AsNoTracking()
                .Where(i => i.Status == (int)StatusInscricao.Pendente);

            if (poloId.HasValue) query = query.Where(i => i.PoloId == poloId.Value);

            return await query.CountAsync();
        }

        public async Task<int> ContarEnviosRecentesAsync(string whatsApp, int minutos)
        {
            var limite = DateTime.UtcNow.AddMinutes(-minutos);
            return await _context.Inscricoes
                .AsNoTracking()
                .CountAsync(i => i.WhatsApp == whatsApp && i.DataEnvio >= limite);
        }

        public async Task AtualizarAsync(Inscricao inscricao)
        {
            _context.Inscricoes.Update(inscricao);
            await _context.SaveChangesAsync();
        }

        public async Task<(Aluno aluno, Matricula matricula)> AprovarAsync(
            Inscricao inscricao, Aluno aluno, Matricula matricula)
        {
            // Aluno, matrícula e baixa da inscrição entram juntos: aprovar pela
            // metade deixaria a fila e o cadastro divergentes.
            using var tx = await _context.Database.BeginTransactionAsync();

            if (aluno.Id > 0)
            {
                _context.Alunos.Update(aluno);
            }
            else
            {
                _context.Alunos.Add(aluno);
            }
            await _context.SaveChangesAsync();

            matricula.AlunoId = aluno.Id;

            // Rematrícula no mesmo ano atualiza a existente em vez de duplicar.
            var existente = await _context.Matriculas
                .FirstOrDefaultAsync(m => m.AlunoId == aluno.Id && m.Ano == matricula.Ano);

            if (existente == null)
            {
                _context.Matriculas.Add(matricula);
            }
            else
            {
                existente.PoloId = matricula.PoloId;
                existente.Turma = matricula.Turma;
                existente.InscricaoId = matricula.InscricaoId;
                existente.DataMatricula = matricula.DataMatricula;
                existente.Ativa = true;
                existente.DataEncerramento = null;
                existente.MotivoEncerramento = string.Empty;
                matricula = existente;
            }

            inscricao.AlunoId = aluno.Id;
            _context.Inscricoes.Update(inscricao);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return (aluno, matricula);
        }

        public async Task<List<Matricula>> ListarMatriculasAsync(int ano, long? poloId)
        {
            var query = _context.Matriculas.AsNoTracking().Where(m => m.Ano == ano);
            if (poloId.HasValue) query = query.Where(m => m.PoloId == poloId.Value);
            return await query.ToListAsync();
        }

        public async Task<Matricula> ObterMatriculaAsync(long alunoId, int ano)
        {
            return await _context.Matriculas
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.AlunoId == alunoId && m.Ano == ano);
        }
    }
}
