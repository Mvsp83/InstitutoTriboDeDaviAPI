using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> Create(UsuarioDTO usuarioDTO);
        Task<UsuarioDTO> Update(UsuarioDTO usuarioDTO);
        Task Delete(long id);
        Task<UsuarioDTO> Get(long id);
        Task<List<UsuarioDTO>> GetAll();
        Task<UsuarioDTO> GetByEmail(string email);
        Task<List<UsuarioDTO>> SearchEmail(string email);
    }
}
