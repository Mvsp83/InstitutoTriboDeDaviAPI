using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModeloDeAulaController : BaseController
    {
        private readonly IModeloDeAulaService _modeloService;
        private readonly ILogger<ModeloDeAulaController> _logger;

        public ModeloDeAulaController(IModeloDeAulaService modeloService, ILogger<ModeloDeAulaController> logger) : base(logger)
        {
            _modeloService = modeloService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allModelos = await _modeloService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Modelos de Aula encontrados com sucesso!",
                    Success = true,
                    Data = allModelos
                });
            });
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var modelo = await _modeloService.Get(id);

                if (modelo == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Modelo de Aula foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Modelo de Aula encontrado com sucesso!",
                    Success = true,
                    Data = modelo
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Create([FromBody] ModeloDeAulaDTO modeloDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var modeloCreated = await _modeloService.Create(modeloDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Modelo de Aula criado com sucesso!",
                    Success = true,
                    Data = modeloCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] ModeloDeAulaDTO modeloDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var modeloUpdated = await _modeloService.Update(modeloDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Modelo de Aula atualizado com sucesso!",
                    Success = true,
                    Data = modeloUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var modelo = await _modeloService.Get(id);

                if (modelo == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Modelo de Aula foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _modeloService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Modelo de Aula removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
