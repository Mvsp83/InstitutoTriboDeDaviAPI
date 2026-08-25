using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using InstitutoTriboDeDavi.API.Utilities;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Administrador))]
    [RequireModulo("financeiro")]
    public class BemPatrimonialController : BaseController
    {
        private readonly IBemPatrimonialService _service;
        private readonly ILogger<BemPatrimonialController> _logger;

        public BemPatrimonialController(
            IBemPatrimonialService service,
            ILogger<BemPatrimonialController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var bens = await _service.GetAll();
                return Ok(new ResultViewModel
                {
                    Message = "Bens obtidos com sucesso!",
                    Success = true,
                    Data = bens
                });
            });
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var bem = await _service.Get(id);
                return Ok(new ResultViewModel
                {
                    Message = bem == null ? "Não encontrado." : "Encontrado!",
                    Success = true,
                    Data = bem
                });
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BemPatrimonialDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var criado = await _service.Create(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Bem cadastrado com sucesso!",
                    Success = true,
                    Data = criado
                });
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] BemPatrimonialDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var atualizado = await _service.Update(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Bem atualizado com sucesso!",
                    Success = true,
                    Data = atualizado
                });
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Delete(id);
                return Ok(new ResultViewModel
                {
                    Message = "Bem removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
