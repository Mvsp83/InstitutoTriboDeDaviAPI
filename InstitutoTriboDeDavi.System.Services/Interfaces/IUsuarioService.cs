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
        Task<UsuarioDTO> GetByNome(string nome);
        Task<List<UsuarioDTO>> SearchByEmail(string email);
        Task<List<UsuarioDTO>> SearchByNome(string nome);
        Task<UsuarioDTO> ValidarUsuarioAsync(string login, string password);
        Task<List<UsuarioDTO>> ObterUsuariosPorTurmaAsync(UsuarioDTO usuario, List<int> turmas);
    }
}
