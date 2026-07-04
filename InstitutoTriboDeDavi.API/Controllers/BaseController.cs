using InstitutoTriboDeDavi.API.Token.Auth;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly ILogger _logger;

        public BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected UsuarioDTO UsuarioAutenticado => Authorization.ObterUsuarioAutenticado(User);

        // Autorização por role é feita via [Authorize(Roles/Policy)] nos endpoints.
        // Aqui fica só a autorização baseada em recurso (polo), que atributo não cobre.

        // Verifica se o usuário pertence ao polo — Administrador tem bypass
        protected void ValidatePoloUsuario(long poloId)
        {
            if (UsuarioAutenticado.Role == UserRole.Administrador)
                return;

            if (UsuarioAutenticado.PoloId != poloId)
                throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
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
                return StatusCode(StatusCodes.Status403Forbidden, Responses.DomainErrorMessage(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }
    }
}
