using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess.Business
{
    public class PresencaRepository : BaseRepository<Presenca>, IPresencaRepository
    {
        private readonly TriboDeDaviContext _context;

        public PresencaRepository(TriboDeDaviContext context) : base(context) 
        {
            _context = context;
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
