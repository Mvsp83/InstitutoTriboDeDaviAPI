using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class OcorrenciaAlunoRepository : IOcorrenciaAlunoRepository
    {
        private readonly TriboDeDaviContext _context;

        public OcorrenciaAlunoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<OcorrenciaAluno>> ListarPorAlunoAsync(long alunoId)
        {
            return await _context.OcorrenciasAluno
                .AsNoTracking()
                .Where(o => o.AlunoId == alunoId)
                .OrderByDescending(o => o.Data)
                .ThenByDescending(o => o.Id)
                .ToListAsync();
        }

        public async Task<OcorrenciaAluno> ObterAsync(long id)
        {
            return await _context.OcorrenciasAluno
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<OcorrenciaAluno> CriarAsync(OcorrenciaAluno ocorrencia)
        {
            _context.OcorrenciasAluno.Add(ocorrencia);
            await _context.SaveChangesAsync();
            return ocorrencia;
        }

        public async Task<bool> ExcluirAsync(long id)
        {
            var o = await _context.OcorrenciasAluno.FirstOrDefaultAsync(x => x.Id == id);
            if (o == null) return false;
            _context.OcorrenciasAluno.Remove(o);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
