using InstitutoTriboDeDavi.System.DTO.Business;

namespace InstitutoTriboDeDavi.System.Services.Business.Interfaces
{
    public interface IPresencaService
    {
        Task<PresencaDTO> Create(PresencaDTO presencaDTO);
        Task<List<PresencaDTO>> CreateBatch(List<PresencaDTO> presencaDTO);
        Task<PresencaDTO> Update(PresencaDTO presencaDTO);
        Task Delete(long id);
        Task<PresencaDTO> Get(long id);
        Task<List<PresencaDTO>> GetAll();
        Task<List<PresencaDTO>> GetPresencasPorAula(long aulaId);
    }
}
