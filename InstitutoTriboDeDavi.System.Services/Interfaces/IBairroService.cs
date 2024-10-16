using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface IBairroService 
    {
        Task<BairroDTO> Create(BairroDTO bairroDTO);
        Task<BairroDTO> Update(BairroDTO bairroDTO);
        Task Delete(long id);
        Task<BairroDTO> Get(long id);
        Task<List<BairroDTO>> GetAll();
        Task<BairroDTO> GetByNome(string nome);
        Task<List<BairroDTO>> SearchByNome(string nome);
    }
}
