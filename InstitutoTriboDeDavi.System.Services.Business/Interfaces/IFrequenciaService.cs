using InstitutoTriboDeDavi.System.DTO.Queries;

namespace InstitutoTriboDeDavi.System.Services.Business.Interfaces
{
    public interface IFrequenciaService
    {
        Task<List<FrequenciaDTO>> GetAlunosFaltas(long poloId);
    }
}
