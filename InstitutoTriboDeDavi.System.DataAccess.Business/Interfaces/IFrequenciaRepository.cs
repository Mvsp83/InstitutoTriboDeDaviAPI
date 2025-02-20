using InstitutoTriboDeDavi.System.Domain.Entities.Consultas;

namespace InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces
{
    public interface IFrequenciaRepository
    {
        Task<List<Frequencia>> GetAlunosFaltasQueryTotal();
        Task<List<Frequencia>> GetAlunosFaltasQuery(long? poloId);
    }
}
