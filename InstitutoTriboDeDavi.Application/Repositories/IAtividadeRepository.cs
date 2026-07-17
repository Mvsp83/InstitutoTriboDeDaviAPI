using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Entities.Consultas;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IAtividadeRepository : IBaseRepository<Atividade>
    {
        Task<List<HistoricoAtividade>> ObterHistoricoTurmaAsync(long poloId, int turma);
    }
}
