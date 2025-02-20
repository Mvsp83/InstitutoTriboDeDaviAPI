using InstitutoTriboDeDavi.System.DTO;

namespace InstitutoTriboDeDavi.API.Token.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(UsuarioDTO usuarioDTO);
    }
}
