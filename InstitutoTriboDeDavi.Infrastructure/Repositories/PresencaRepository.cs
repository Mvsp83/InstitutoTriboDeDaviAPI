using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class PresencaRepository : BaseRepository<Presenca>, IPresencaRepository
    {
        private readonly TriboDeDaviContext _context;

        public PresencaRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Presenca>> CreateBatchComAulaAsync(IEnumerable<Presenca> presencas, long aulaId)
        {
            var lista = presencas.ToList();

            // Carrega a aula rastreada para que o flag PresencaSalva seja
            // gravado no mesmo SaveChanges (transação única) das presenças
            var aula = await _context.Aulas.FirstOrDefaultAsync(a => a.Id == aulaId);
            if (aula != null)
                aula.PresencaSalva = true;

            await _context.Presencas.AddRangeAsync(lista);
            await _context.SaveChangesAsync();

            return lista;
        }

        public async Task<List<Presenca>> ObterPorAlunoAsync(long alunoId)
        {
            return await _context.Presencas
                .Where(p => p.AlunoId == alunoId)
                .OrderByDescending(p => p.Data)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Presenca>> GetPresencasPorAula(long aulaId)
        {
            var query = from presenca in _context.Presencas
                        join aluno in _context.Alunos
                        on presenca.AlunoId equals aluno.Id
                        where presenca.AulaId == aulaId
                        select new Presenca
                        {
                            Id = presenca.Id,
                            AulaId = presenca.AulaId,
                            AlunoId = presenca.AlunoId,
                            NomeAluno = aluno.Nome,
                            EstaPresente = presenca.EstaPresente,
                            Observacoes = presenca.Observacoes,
                            Data = presenca.Data
                        };

            return await query.ToListAsync();
        }
    }
}
