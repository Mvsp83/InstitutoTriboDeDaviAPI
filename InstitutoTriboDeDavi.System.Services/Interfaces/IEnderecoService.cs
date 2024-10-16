using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface IEnderecoService 
    {
        Task<EnderecoDTO> Create(EnderecoDTO enderecoDTO);
        Task<EnderecoDTO> Update(EnderecoDTO enderecoDTO);
        Task Delete(long id);
        Task<EnderecoDTO> Get(long id);
        Task<List<EnderecoDTO>> GetAll();
        Task<EnderecoDTO> GetByNome(string nome);
        Task<List<EnderecoDTO>> SearchByNome(string nome);
    }
}
