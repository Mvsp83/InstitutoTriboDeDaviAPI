using InstitutoTriboDeDavi.API.Token.Auth;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Business;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly ILogger<Controller> _logger;

        public BaseController(ILogger<Controller> logger)
        {
            _logger = logger;
        }

        protected UsuarioDTO UsuarioAutenticado => Authorization.ObterUsuarioAutenticado(User);

        protected void ValidateUserRole(UserRole requiredRole)
        {
            if (UsuarioAutenticado.Role != requiredRole)
            {
                throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
            }
        }

        protected void ValidatePoloUsuario(UsuarioDTO user, long poloId)
        {
            if (UsuarioAutenticado.PoloId != poloId)
            {
                throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
            }
        }

        protected async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, ex.Message);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }
    }
}
