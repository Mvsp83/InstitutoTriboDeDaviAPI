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
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ITokenGenerator tokenGenerator, IMapper mapper, IUsuarioService usuarioService, IRefreshTokenService refreshTokenService, ILogger<AuthController> logger) : base(logger)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _usuarioService = usuarioService;
            _refreshTokenService = refreshTokenService;
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

                // Segundo fator, quando o usuário tem 2FA ativo.
                if (usuario.TotpConfirmado)
                {
                    if (string.IsNullOrWhiteSpace(loginViewModel.Codigo2fa))
                    {
                        // Senha certa, mas falta o código: primeira etapa concluída.
                        return Ok(new ResultViewModel
                        {
                            Message = "Informe o código do aplicativo autenticador.",
                            Success = true,
                            Data = new { Requer2fa = true }
                        });
                    }

                    if (!await _usuarioService.ValidarCodigo2FAAsync(loginViewModel.Login, loginViewModel.Codigo2fa))
                    {
                        _logger.LogWarning("Código 2FA inválido no login de {Login}", loginViewModel.Login);
                        return StatusCode(401, Responses.UnauthorizedErrorMessage());
                    }
                }

                var token = _tokenGenerator.GenerateToken(usuario);
                var refreshToken = await _refreshTokenService.EmitirAsync(usuario.Id);

                return Ok(new ResultViewModel
                {
                    Message = "Usuário autenticado com sucesso!",
                    Success = true,
                    Data = new
                    {
                        Token = token,
                        TokenExpires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:HoursToExpire"])),
                        RefreshToken = refreshToken
                    }
                });
            });
        }

        // Renova o access token a partir de um refresh token válido (rotação: o
        // refresh usado é revogado e um novo é emitido). Anônimo por natureza —
        // o access token já expirou quando se chega aqui.
        [HttpPost]
        [Route("/api/v1/auth/refresh")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.LoginRateLimit)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenViewModel viewModel)
        {
            return await ExecuteAsync(async () =>
            {
                var rotacao = await _refreshTokenService.RotacionarAsync(viewModel?.RefreshToken);
                if (rotacao == null)
                    return StatusCode(401, Responses.UnauthorizedErrorMessage());

                // Usuário pode ter sido excluído desde a emissão — sem ele, sem token.
                var usuario = await _usuarioService.Get(rotacao.UsuarioId);
                if (usuario == null)
                    return StatusCode(401, Responses.UnauthorizedErrorMessage());

                var token = _tokenGenerator.GenerateToken(usuario);

                return Ok(new ResultViewModel
                {
                    Message = "Sessão renovada.",
                    Success = true,
                    Data = new
                    {
                        Token = token,
                        TokenExpires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:HoursToExpire"])),
                        RefreshToken = rotacao.NovoTokenRaw
                    }
                });
            });
        }

        // Encerra a sessão revogando o refresh token no servidor.
        [HttpPost]
        [Route("/api/v1/auth/logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenViewModel viewModel)
        {
            return await ExecuteAsync(async () =>
            {
                await _refreshTokenService.RevogarAsync(viewModel?.RefreshToken);

                return Ok(new ResultViewModel
                {
                    Message = "Sessão encerrada.",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
