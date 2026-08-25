using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class AptidaoGraduacaoRepository : IAptidaoGraduacaoRepository
    {
        private readonly TriboDeDaviContext _context;

        public AptidaoGraduacaoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<AptidaoGraduacaoDTO>> ObterAsync(long? poloId)
        {
            var alunosQ = _context.Alunos.AsNoTracking().AsQueryable();
            if (poloId.HasValue) alunosQ = alunosQ.Where(a => a.PoloId == poloId.Value);
            var alunos = await alunosQ
                .Select(a => new { a.Id, a.Faixa, a.PoloId })
                .ToListAsync();

            var gradQ = _context.Graduacoes.AsNoTracking().AsQueryable();
            if (poloId.HasValue) gradQ = gradQ.Where(g => g.PoloId == poloId.Value);
            var ultimaGrad = (await gradQ
                .GroupBy(g => g.AlunoId)
                .Select(x => new { AlunoId = x.Key, Data = x.Max(g => g.Data) })
                .ToListAsync())
                .ToDictionary(x => x.AlunoId, x => x.Data);

            var presQ = _context.Presencas.AsNoTracking().Where(p => p.EstaPresente);
            if (poloId.HasValue) presQ = presQ.Where(p => p.PoloId == poloId.Value);
            var presencas = (await presQ
                .Select(p => new { p.AlunoId, p.Data })
                .ToListAsync())
                .GroupBy(p => p.AlunoId)
                .ToDictionary(g => g.Key, g => g.Select(p => p.Data).ToList());

            var ocQ = _context.OcorrenciasAluno.AsNoTracking().Where(o => o.Tipo == 0);
            if (poloId.HasValue) ocQ = ocQ.Where(o => o.PoloId == poloId.Value);
            var advertencias = (await ocQ
                .Select(o => new { o.AlunoId, o.Data })
                .ToListAsync())
                .GroupBy(o => o.AlunoId)
                .ToDictionary(g => g.Key, g => g.Select(o => o.Data).ToList());

            var lista = new List<AptidaoGraduacaoDTO>(alunos.Count);
            foreach (var a in alunos)
            {
                DateTime? dataRef = ultimaGrad.TryGetValue(a.Id, out var dataUlt)
                    ? dataUlt
                    : (DateTime?)null;
                var corte = dataRef ?? DateTime.MinValue;

                var pres = presencas.TryGetValue(a.Id, out var pl)
                    ? pl.Count(d => d >= corte)
                    : 0;
                var adv = advertencias.TryGetValue(a.Id, out var ol)
                    ? ol.Count(d => d >= corte)
                    : 0;

                lista.Add(new AptidaoGraduacaoDTO
                {
                    AlunoId = a.Id,
                    Faixa = (int)a.Faixa,
                    PoloId = a.PoloId,
                    DataUltimaGraduacao = dataRef,
                    PresencasDesdeUltima = pres,
                    AdvertenciasDesdeUltima = adv,
                });
            }

            return lista;
        }
    }
}
