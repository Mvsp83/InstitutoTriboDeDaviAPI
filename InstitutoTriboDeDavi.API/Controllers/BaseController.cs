using InstitutoTriboDeDavi.API.Token.Auth;
using InstitutoTriboDeDavi.System.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected UsuarioDTO UsuarioAutenticado => Authorization.ObterUsuarioAutenticado(User);
    }
}
