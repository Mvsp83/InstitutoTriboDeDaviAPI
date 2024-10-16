using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface IResponsavelService 
    {
        Task<ResponsavelDTO> Create(ResponsavelDTO responsavelDTO);
        Task<ResponsavelDTO> Update(ResponsavelDTO responsavelDTO);
        Task Delete(long id);
        Task<ResponsavelDTO> Get(long id);
        Task<List<ResponsavelDTO>> GetAll();
        Task<ResponsavelDTO> GetByNome(string nome);
        Task<List<ResponsavelDTO>> SearchByNome(string nome);
    }
}
