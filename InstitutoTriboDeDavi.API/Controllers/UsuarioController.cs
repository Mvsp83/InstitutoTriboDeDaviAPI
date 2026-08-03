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
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IMapper mapper, IUsuarioService usuarioService, ILogger<UsuarioController> logger) : base(logger)
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
            _logger = logger;
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

        // Endpoint público de bootstrap: só funciona enquanto não existe nenhum usuário
        [HttpPost("setup")]
        [AllowAnonymous]
        public async Task<IActionResult> Setup([FromBody] UsuarioViewModel usuarioViewModel)
        {
            return await ExecuteAsync(async () =>
            {
                var jaExisteUsuario = await _usuarioService.ExisteQualquerUsuario();

                if (jaExisteUsuario)
                    return Forbid();

                usuarioViewModel.Role = UserRole.Administrador;

                var usuarioCreated = await _usuarioService.Create(_mapper.Map<UsuarioDTO>(usuarioViewModel));

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
    }
}
