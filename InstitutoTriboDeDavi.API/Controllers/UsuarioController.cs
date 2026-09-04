using AutoMapper;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.API.ViewModels.Usuario;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<UsuarioController> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioController(IMapper mapper, IUsuarioService usuarioService, IRefreshTokenService refreshTokenService, ILogger<UsuarioController> logger, IConfiguration configuration) : base(logger)
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Create([FromBody] UsuarioViewModel usuarioViewModel)
        {
            return await ExecuteAsync(async () =>
            {
                var usuarioCreated = await _usuarioService.Create(_mapper.Map<UsuarioDTO>(usuarioViewModel));

                return Ok(new ResultViewModel
                {
                    Message = "Usuário criado com sucesso!",
                    Success = true,
                    Data = usuarioCreated
                });
            });
        }

        // Endpoint público de bootstrap: cria o primeiro admin e só funciona
        // enquanto não existe nenhum usuário. Em produção, defina "Setup:Token"
        // (env Setup__Token) para exigir um segredo e fechar a janela em que o
        // banco vazio ficaria aberto a qualquer um.
        [HttpPost("setup")]
        [AllowAnonymous]
        public async Task<IActionResult> Setup([FromBody] PrimeiroAcessoViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                var jaExisteUsuario = await _usuarioService.ExisteQualquerUsuario();

                if (jaExisteUsuario)
                    return Forbid();

                var tokenConfigurado = _configuration["Setup:Token"];
                if (!string.IsNullOrWhiteSpace(tokenConfigurado) &&
                    !string.Equals(tokenConfigurado, model?.Token, StringComparison.Ordinal))
                    return Forbid();

                var novoAdmin = new UsuarioViewModel
                {
                    Login = model.Login,
                    Email = model.Email,
                    Password = model.Password,
                    Role = UserRole.Administrador
                };

                var usuarioCreated = await _usuarioService.Create(_mapper.Map<UsuarioDTO>(novoAdmin));

                return Ok(new ResultViewModel
                {
                    Message = "Usuário administrador criado com sucesso!",
                    Success = true,
                    Data = usuarioCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Update([FromBody] UsuarioUpdateDTO usuarioDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var usuarioUpdated = await _usuarioService.Update(usuarioDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Usuário atualizado com sucesso!",
                    Success = true,
                    Data = usuarioUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var usuario = await _usuarioService.Get(id);

                if (usuario == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _usuarioService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Usuário removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var usuario = await _usuarioService.Get(id);

                if (usuario == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Usuário encontrado com sucesso!",
                    Success = true,
                    Data = usuario
                });
            });
        }

        [HttpGet("get-all")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allUsuarios = await _usuarioService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Usuários encontrados com sucesso!",
                    Success = true,
                    Data = allUsuarios
                });
            });
        }

        [HttpGet("get-by-email")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            return await ExecuteAsync(async () =>
            {
                var usuario = await _usuarioService.GetByEmail(email);

                if (usuario == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o Email informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Usuário encontrado com sucesso!",
                    Success = true,
                    Data = usuario
                });
            });
        }

        [HttpGet("search-by-email")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> SearchByEmail([FromQuery] string email)
        {
            return await ExecuteAsync(async () =>
            {
                var allUsuarios = await _usuarioService.SearchByEmail(email);

                if (!allUsuarios.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o Email informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Usuários encontrados com sucesso!",
                    Success = true,
                    Data = allUsuarios
                });
            });
        }

        [HttpGet("get-by-nome")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            return await ExecuteAsync(async () =>
            {
                var usuario = await _usuarioService.GetByNome(nome);

                if (usuario == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Usuário encontrado com sucesso!",
                    Success = true,
                    Data = usuario
                });
            });
        }

        [HttpGet("search-by-nome")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> SearchByNome([FromQuery] string nome)
        {
            return await ExecuteAsync(async () =>
            {
                var allUsuarios = await _usuarioService.SearchByNome(nome);

                if (!allUsuarios.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Usuários encontrados com sucesso!",
                    Success = true,
                    Data = allUsuarios
                });
            });
        }

        // Avatar do próprio usuário autenticado (qualquer papel).
        [HttpGet("meu-avatar")]
        [Authorize]
        public async Task<IActionResult> ObterMeuAvatar()
        {
            return await ExecuteAsync(async () =>
            {
                var avatar = await _usuarioService.ObterAvatarAsync(UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Avatar obtido com sucesso!",
                    Success = true,
                    Data = new { avatar }
                });
            });
        }

        [HttpPut("meu-avatar")]
        [Authorize]
        public async Task<IActionResult> AtualizarMeuAvatar([FromBody] AtualizarAvatarViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                await _usuarioService.AtualizarAvatarAsync(UsuarioAutenticado.Login, model.Avatar);

                return Ok(new ResultViewModel
                {
                    Message = "Avatar atualizado com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // Perfil público do professor (foto de rosto + faixa na seção do polo).
        [HttpGet("meu-perfil-site")]
        [Authorize]
        public async Task<IActionResult> ObterMeuPerfilSite()
        {
            return await ExecuteAsync(async () =>
            {
                var perfil = await _usuarioService.ObterMeuPerfilSiteAsync(UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Perfil obtido com sucesso!",
                    Success = true,
                    Data = perfil
                });
            });
        }

        [HttpPut("meu-perfil-site")]
        [Authorize]
        public async Task<IActionResult> AtualizarMeuPerfilSite([FromBody] PerfilSiteDTO model)
        {
            return await ExecuteAsync(async () =>
            {
                await _usuarioService.AtualizarMeuPerfilSiteAsync(UsuarioAutenticado.Login, model);

                return Ok(new ResultViewModel
                {
                    Message = "Perfil atualizado com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet("get-por-polo")]
        [Authorize]
        public async Task<IActionResult> ObterUsuarios([FromQuery] List<int> turmas)
        {
            return await ExecuteAsync(async () =>
            {
                var allUsuarios = await _usuarioService.ObterUsuariosPorTurmaAsync(UsuarioAutenticado, turmas);

                if (!allUsuarios.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Usuários obtidos com sucesso!",
                    Success = true,
                    Data = allUsuarios
                });
            });
        }

        // ── 2FA (TOTP) do próprio usuário autenticado ───────────────────────

        [HttpGet("2fa/status")]
        [Authorize]
        public async Task<IActionResult> Status2FA()
        {
            return await ExecuteAsync(async () =>
            {
                var ativo = await _usuarioService.Status2FAAsync(UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Status do 2FA obtido.",
                    Success = true,
                    Data = new { ativo }
                });
            });
        }

        // Gera o secret e a URI para o QR. O 2FA só passa a valer após confirmar.
        [HttpPost("2fa/iniciar")]
        [Authorize]
        public async Task<IActionResult> Iniciar2FA()
        {
            return await ExecuteAsync(async () =>
            {
                var setup = await _usuarioService.Iniciar2FAAsync(UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Escaneie o QR no app autenticador e confirme com um código.",
                    Success = true,
                    Data = setup
                });
            });
        }

        [HttpPost("2fa/confirmar")]
        [Authorize]
        public async Task<IActionResult> Confirmar2FA([FromBody] Codigo2FAViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                await _usuarioService.Confirmar2FAAsync(UsuarioAutenticado.Login, model.Codigo);

                _logger.LogInformation("2FA ativado para {Login}", UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "2FA ativado com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpPost("2fa/desativar")]
        [Authorize]
        public async Task<IActionResult> Desativar2FA([FromBody] Codigo2FAViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                await _usuarioService.Desativar2FAAsync(UsuarioAutenticado.Login, model.Codigo);

                _logger.LogWarning("2FA desativado para {Login}", UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "2FA desativado.",
                    Success = true,
                    Data = null
                });
            });
        }

        // Revoga todas as sessões (refresh tokens) de um usuário — "sair de
        // todos os aparelhos" / ao desativar alguém. Só Administrador.
        [HttpPost("{id}/revogar-sessoes")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> RevogarSessoes(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var quantidade = await _refreshTokenService.RevogarUsuarioAsync(id);

                _logger.LogWarning("{Quantidade} sessão(ões) do usuário #{Id} revogada(s) por {Admin}.", quantidade, id, UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = $"{quantidade} sessão(ões) revogada(s).",
                    Success = true,
                    Data = new { revogadas = quantidade }
                });
            });
        }
    }
}
