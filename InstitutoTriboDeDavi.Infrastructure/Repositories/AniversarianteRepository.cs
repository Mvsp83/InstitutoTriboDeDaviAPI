using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.DTO.Queries;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
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
            var mesAtual = hoje.Month;
            var diaAtual = hoje.Day;
            var lista = await _context.Alunos
                .Where(a => a.DataNascimento.Month == mes)
                .OrderBy(a => a.DataNascimento.Day)
                .Select(a => new AniversarianteDTO
                {
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    // "Já comemorado" = o aniversário deste ano já passou. Antes
                    // comparava só o dia (ignorando o mês), então meses passados
                    // apareciam como não comemorados. (mes = mês consultado.)
                    JaComemorado = mes < mesAtual
                        || (mes == mesAtual && a.DataNascimento.Day < diaAtual)
                })
                .ToListAsync();

            return lista;
        }

        public async Task<List<AniversarianteDTO>> GetAniversariantesPorPoloAsync(int mes, long idPolo)
        {
            var hoje = DateTime.Now;
            var mesAtual = hoje.Month;
            var diaAtual = hoje.Day;
            var lista = await _context.Alunos
                .Where(a => a.DataNascimento.Month == mes && a.PoloId == idPolo)
                .OrderBy(a => a.DataNascimento.Day)
                .Select(a => new AniversarianteDTO
                {
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    // "Já comemorado" = o aniversário deste ano já passou. Antes
                    // comparava só o dia (ignorando o mês), então meses passados
                    // apareciam como não comemorados. (mes = mês consultado.)
                    JaComemorado = mes < mesAtual
                        || (mes == mesAtual && a.DataNascimento.Day < diaAtual)
                })
                .ToListAsync();

            return lista;
        }
    }
}
