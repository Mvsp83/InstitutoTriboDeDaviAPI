using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IPresencaRepository : IBaseRepository<Presenca>
    {
        Task<List<Presenca>> GetPresencasPorAula(long aulaId);

        // Histórico de presenças de um aluno (portal do responsável / frequência).
        Task<List<Presenca>> ObterPorAlunoAsync(long alunoId);

        /// Salva o lote de presenças e marca a aula como salva na MESMA
        /// transação — evita aula "pendente" com presenças já gravadas.
        Task<List<Presenca>> CreateBatchComAulaAsync(IEnumerable<Presenca> presencas, long aulaId);
    }
}
