using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.API.Token.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(UsuarioDTO usuarioDTO);
    }
}
