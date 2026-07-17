using AutoMapper;
using InstitutoTriboDeDavi.API.Token.Interfaces;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ITokenGenerator tokenGenerator, IMapper mapper, IUsuarioService usuarioService, ILogger<AuthController> logger) : base(logger)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/api/v1/auth/login")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.LoginRateLimit)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            return await ExecuteAsync(async () =>
            {
                var usuario = await _usuarioService.ValidarUsuarioAsync(loginViewModel.Login, loginViewModel.Password);

                if (usuario == null)
                {
                    // Auditoria de tentativas falhas (força bruta, senha errada)
                    _logger.LogWarning("Tentativa de login malsucedida para o login {Login}", loginViewModel.Login);
                    return StatusCode(401, Responses.UnauthorizedErrorMessage());
                }

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
            });
        }

    }
}

