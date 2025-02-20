using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.DTO.Queries;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess.Business
{
    public class AniversarianteRepository : IAniversarianteRepository
    {
        private readonly TriboDeDaviContext _context;

        public AniversarianteRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<AniversarianteDTO>> GetAniversariantesAsync(int mes)
        {
            var hoje = DateTime.Now;
            var lista = await _context.Alunos
                .Where(a => a.DataNascimento.Month == mes)
                .OrderBy(a => a.DataNascimento.Day)
                .Select(a => new AniversarianteDTO
                {
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    JaComemorado = a.DataNascimento.Day < hoje.Day
                })
                .ToListAsync();

            return lista;
        }

        public async Task<List<AniversarianteDTO>> GetAniversariantesPorPoloAsync(int mes, long idPolo)
        {
            var hoje = DateTime.Now;
            var lista = await _context.Alunos
                .Where(a => a.DataNascimento.Month == mes && a.PoloId == idPolo)
                .OrderBy(a => a.DataNascimento.Day)
                .Select(a => new AniversarianteDTO
                {
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    JaComemorado = a.DataNascimento.Day < hoje.Day
                })
                .ToListAsync();

            return lista;
        }
    }
}
