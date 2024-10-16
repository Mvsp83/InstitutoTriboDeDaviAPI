using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface IEstadoService 
    {
        Task<EstadoDTO> Create(EstadoDTO estadoDTO);
        Task<EstadoDTO> Update(EstadoDTO estadoDTO);
        Task Delete(long id);
        Task<EstadoDTO> Get(long id);
        Task<List<EstadoDTO>> GetAll();
        Task<EstadoDTO> GetByNome(string nome);
        Task<List<EstadoDTO>> SearchByNome(string nome);
    }
}
