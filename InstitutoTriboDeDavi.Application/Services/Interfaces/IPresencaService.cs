using InstitutoTriboDeDavi.Application.DTO.Business;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IPresencaService
    {
        Task<PresencaDTO> Create(PresencaDTO presencaDTO);
        Task<List<PresencaDTO>> CreateBatch(IEnumerable<PresencaDTO> presencaDTO);
        Task<PresencaDTO> Update(PresencaDTO presencaDTO);
        Task Delete(long id);
        Task<PresencaDTO> Get(long id);
        Task<List<PresencaDTO>> GetAll();
        Task<List<PresencaDTO>> GetPresencasPorAula(long aulaId);
    }
}
