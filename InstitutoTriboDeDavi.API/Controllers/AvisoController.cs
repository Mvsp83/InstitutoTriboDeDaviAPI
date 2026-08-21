using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvisoController : BaseController
    {
        private readonly IAvisoService _service;
        private readonly ILogger<AvisoController> _logger;

        public AvisoController(IAvisoService service, ILogger<AvisoController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // Avisos pendentes do usuário logado (mostrados no login).
        [HttpGet("pendentes")]
        public async Task<IActionResult> Pendentes()
        {
            return await ExecuteAsync(async () =>
            {
                var avisos = await _service.ObterPendentes(UsuarioAutenticado);
                return Ok(new ResultViewModel
                {
                    Message = "Avisos pendentes.",
                    Success = true,
                    Data = avisos
                });
            });
        }

        // "Ciente": para de aparecer para este usuário.
        [HttpPost("ciente/{avisoId}")]
        public async Task<IActionResult> Ciente(long avisoId)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.MarcarCiente(avisoId, UsuarioAutenticado);
                return Ok(new ResultViewModel
                {
                    Message = "Aviso marcado como ciente.",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet("get-all")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var avisos = await _service.GetAll();
                return Ok(new ResultViewModel
                {
                    Message = "Avisos obtidos com sucesso!",
                    Success = true,
                    Data = avisos
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Create([FromBody] AvisoDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var criado = await _service.Create(dto, UsuarioAutenticado);
                return Ok(new ResultViewModel
                {
                    Message = "Aviso publicado com sucesso!",
                    Success = true,
                    Data = criado
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Delete(id);
                return Ok(new ResultViewModel
                {
                    Message = "Aviso removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
