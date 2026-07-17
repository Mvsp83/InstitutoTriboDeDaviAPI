using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Entities.Consultas;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class AtividadeRepository : BaseRepository<Atividade>, IAtividadeRepository
    {
        private readonly TriboDeDaviContext _context;

        public AtividadeRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        // Última vez (e quantas vezes) que cada atividade apareceu
        // nos planos de aula de um polo/turma
        public async Task<List<HistoricoAtividade>> ObterHistoricoTurmaAsync(long poloId, int turma)
        {
            return await (from vinculo in _context.AtividadesDoBloco
                          join bloco in _context.BlocosDoPlano on vinculo.BlocoDoPlanoId equals bloco.Id
                          join plano in _context.PlanosDeAula on bloco.PlanoDeAulaId equals plano.Id
                          join atividade in _context.Atividades on vinculo.AtividadeId equals atividade.Id
                          where plano.PoloId == poloId && plano.Turma == turma
                          group plano by new { atividade.Id, atividade.Nome, atividade.Tipo } into grupo
                          select new HistoricoAtividade
                          {
                              AtividadeId = grupo.Key.Id,
                              Nome = grupo.Key.Nome,
                              Tipo = grupo.Key.Tipo,
                              UltimaData = grupo.Max(p => p.DataPrevista),
                              Vezes = grupo.Count()
                          })
                          .OrderByDescending(h => h.UltimaData)
                          .ToListAsync();
        }
    }
}
