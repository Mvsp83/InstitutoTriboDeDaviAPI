using InstitutoTriboDeDavi.System.DTO.Business;

namespace InstitutoTriboDeDavi.System.Services.Business.Interfaces
{
    public interface IAulaService
    {
        Task<AulaDTO> Create(AulaDTO aulaDTO);
        Task<AulaDTO> Update(AulaDTO aulaDTO);
        Task Delete(long id);
        Task<AulaDTO> Get(long id);
        Task<List<AulaDTO>> GetAll();
    }
}
