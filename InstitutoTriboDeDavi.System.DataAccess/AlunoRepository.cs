using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
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
    }
}
