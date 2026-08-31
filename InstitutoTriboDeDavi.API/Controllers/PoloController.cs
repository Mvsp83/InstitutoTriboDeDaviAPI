using AutoMapper;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoloController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IPoloService _poloService;
        private readonly ILogger<PoloController> _logger;

        public PoloController(IMapper mapper, IPoloService poloService, ILogger<PoloController> logger) : base(logger)
        {
            _mapper = mapper;
            _poloService = poloService;
            _logger = logger;
        }

        // Público: usado na página de Informações do site para listar os polos
        // com endereço e horários (dados do próprio cadastro).
        [HttpGet("publicos")]
        [AllowAnonymous]
        public async Task<IActionResult> Publicos()
        {
            return await ExecuteAsync(async () => new OkObjectResult(new ResultViewModel
            {
                Message = "Polos.",
                Success = true,
                Data = await _poloService.ListarPublicos()
            }));
        }

        [HttpGet("get-por-polo")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] List<int> turmas)
        {
            return await ExecuteAsync(async () =>
            {
                var allPolos = await _poloService.ObterPolosAsync(UsuarioAutenticado, turmas);

                if (!allPolos.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Polo foi encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Polos obtidos com sucesso!",
                    Success = true,
                    Data = allPolos
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Create([FromBody] PoloDTO poloDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var poloCreated = await _poloService.Create(poloDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Polo criado com sucesso!",
                    Success = true,
                    Data = poloCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Update([FromBody] PoloDTO poloDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var poloUpdated = await _poloService.Update(poloDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Polo atualizado com sucesso!",
                    Success = true,
                    Data = poloUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var polo = await _poloService.Get(id);

                if (polo == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Polo foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _poloService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Polo removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var polo = await _poloService.Get(id);

                if (polo == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Polo foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Polo encontrado com sucesso!",
                    Success = true,
                    Data = polo
                });
            });
        }
    }
}
