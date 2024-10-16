using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface IPaisService
    {
        Task<PaisDTO> Create(PaisDTO paisDTO);
        Task<PaisDTO> Update(PaisDTO paisDTO);
        Task Delete(long id);
        Task<PaisDTO> Get(long id);
        Task<List<PaisDTO>> GetAll();
        Task<PaisDTO> GetByNome(string nome);
        Task<List<PaisDTO>> SearchByNome(string nome);
    }
}
