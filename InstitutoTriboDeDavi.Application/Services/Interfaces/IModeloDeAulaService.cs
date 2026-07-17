using InstitutoTriboDeDavi.Application.DTO.Business;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IModeloDeAulaService
    {
        Task<ModeloDeAulaDTO> Create(ModeloDeAulaDTO modeloDTO);
        Task<ModeloDeAulaDTO> Update(ModeloDeAulaDTO modeloDTO);
        Task<ModeloDeAulaDTO> Get(long id);
        Task<List<ModeloDeAulaDTO>> GetAll();
        Task Delete(long id);
    }
}
