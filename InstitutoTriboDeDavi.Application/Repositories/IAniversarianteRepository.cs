using InstitutoTriboDeDavi.Application.DTO.Queries;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IAniversarianteRepository
    {      
        Task<List<AniversarianteDTO>> GetAniversariantesAsync(int mes);
        Task<List<AniversarianteDTO>> GetAniversariantesPorPoloAsync(int mes, long idPolo);
    }
}
