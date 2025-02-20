using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.DTO;
using System.Security.Claims;

namespace InstitutoTriboDeDavi.API.Token.Auth
{
    public class Authorization
    {
        public static UsuarioDTO ObterUsuarioAutenticado(ClaimsPrincipal user)
        {
            if (user.Identity is not ClaimsIdentity claimsIdentity || !claimsIdentity.IsAuthenticated)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var login = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
            var poloId = claimsIdentity.FindFirst("PoloId")?.Value;

            return new UsuarioDTO
            {
                Email = email,
                Login = login,
                Role = Enum.TryParse<UserRole>(role, out var parsedRole) ? parsedRole : UserRole.Default,
                PoloId = string.IsNullOrEmpty(poloId) ? (long?)null : long.Parse(poloId)
            };
        }

    }
}
