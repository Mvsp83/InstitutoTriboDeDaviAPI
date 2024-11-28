using InstitutoTriboDeDavi.System.DTO.Queries;

namespace InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces
{
    public interface IAniversarianteRepository
    {
        Task<List<AniversarianteDTO>> GetAniversariantesAsync(int mes);
    }
}
