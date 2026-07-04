using AutoMapper;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Consultas;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class FrequenciaRepository : IFrequenciaRepository
    {
        private readonly IMapper _mapper;
        private readonly TriboDeDaviContext _context;

        public FrequenciaRepository(TriboDeDaviContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<Frequencia>> GetAlunosFaltasQuery(long? poloId)
        {
            var resultado = await (from presenca in _context.Presencas
                                   join aluno in _context.Alunos on presenca.AlunoId equals aluno.Id
                                   where aluno.PoloId == poloId
                                   group presenca by new { aluno.Nome, aluno.Faixa } into g
                                   select new Frequencia
                                   {
                                       Nome = g.Key.Nome,
                                       Faixa = g.Key.Faixa,
                                       TotalAulas = g.Count(),
                                       TotalFaltas = g.Count(p => !p.EstaPresente)
                                   })
               .OrderByDescending(x => x.TotalFaltas)
               .ToListAsync();

            return resultado;
        }

        public async Task<List<Frequencia>> GetAlunosFaltasQueryTotal()
        {
            var resultado = await (from presenca in _context.Presencas
                                   join aluno in _context.Alunos on presenca.AlunoId equals aluno.Id
                                   group presenca by new { aluno.Nome, aluno.Faixa } into g
                                   select new Frequencia
                                   {
                                       Nome = g.Key.Nome,
                                       Faixa = g.Key.Faixa,
                                       TotalAulas = g.Count(),
                                       TotalFaltas = g.Count(p => !p.EstaPresente)
                                   })
               .OrderByDescending(x => x.TotalFaltas)
               .ToListAsync();

            return resultado;
        }
    }
}
