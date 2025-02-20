using AutoMapper;
using InstitutoTriboDeDavi.API.Token.Interfaces;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;

        public AuthController(IConfiguration configuration, ITokenGenerator tokenGenerator, IMapper mapper, IUsuarioService usuarioService)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _usuarioService = usuarioService;
        }

        [HttpPost]
        [Route("/api/v1/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            try
            {
                var usuario = await _usuarioService.ValidarUsuarioAsync(loginViewModel.Login, loginViewModel.Password);

                if (usuario == null)
                    return StatusCode(401, Responses.UnathorizedErrorMessage());

                var token = _tokenGenerator.GenerateToken(usuario);

                return Ok(new ResultViewModel
                {
                    Message = "Usuário autenticado com sucesso!",
                    Success = true,
                    Data = new
                    {
                        Token = token,
                        TokenExpires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:HoursToExpire"]))
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

    }
}

