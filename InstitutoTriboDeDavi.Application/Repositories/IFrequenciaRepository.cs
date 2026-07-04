using InstitutoTriboDeDavi.Domain.Entities.Consultas;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IFrequenciaRepository
    {
        Task<List<Frequencia>> GetAlunosFaltasQueryTotal();
        Task<List<Frequencia>> GetAlunosFaltasQuery(long? poloId);
    }
}
