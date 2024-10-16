using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface ICidadeService 
    {
        Task<CidadeDTO> Create(CidadeDTO cidadeDTO);
        Task<CidadeDTO> Update(CidadeDTO cidadeDTO);
        Task Delete(long id);
        Task<CidadeDTO> Get(long id);
        Task<List<CidadeDTO>> GetAll();
        Task<CidadeDTO> GetByNome(string nome);
        Task<List<CidadeDTO>> SearchByNome(string nome);
    }
}
