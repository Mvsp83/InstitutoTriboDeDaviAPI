using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class AlunoRepository : BaseRepository<Aluno>, IAlunoRepository
    {
        private readonly TriboDeDaviContext _context;
        public AlunoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Aluno> GetByNome(string nome)
        {
            var aluno = await _context.Alunos
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return aluno.FirstOrDefault();
        }

        public async Task<List<Aluno>> SearchByNome(string nome)
        {
            var allAlunos = await _context.Alunos
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allAlunos;
        }

        public async Task<int> GetTotalAlunosAsync()
        {
            return await _context.Set<Aluno>().CountAsync();
        }

        public async Task<List<Aluno>> ObterTodosAsync()
        {
            return await _context.Alunos.ToListAsync();
        }

        public async Task<List<Aluno>> ObterPorPoloTurmaAsync(long poloId, List<int> turmas)
        {
            return await _context.Alunos
                                 .Where(a => a.PoloId == poloId && turmas.Contains(a.Turma))
                                 .ToListAsync();
        }

        public async Task<List<Aluno>> ObterPendentesPorPoloAsync(long poloId)
        {
            return await _context.Alunos
                .Where(a => a.PoloId == poloId && a.Turma == 0)
                .OrderBy(a => a.Nome)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Aluno>> ObterTodosPendentesAsync()
        {
            return await _context.Alunos
                .Where(a => a.Turma == 0)
                .OrderBy(a => a.PoloId)
                .ThenBy(a => a.Nome)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
