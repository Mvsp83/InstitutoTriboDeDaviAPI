using InstitutoTriboDeDavi.API.Utilities;
using AutoMapper;
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
    public class PresencaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IPresencaService _presencaService;
        private readonly ILogger<PresencaController> _logger;

        public PresencaController(IPresencaService presencaService, IMapper mapper, ILogger<PresencaController> logger) : base(logger)
        {
            _mapper = mapper;
            _presencaService = presencaService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allPresencas = await _presencaService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Presenças encontradas com sucesso!",
                    Success = true,
                    Data = allPresencas
                });
            });
        }

        [HttpPost("batch/create")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> CreateBatch([FromBody] List<PresencaDTO> presencaDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var presencasCreated = await _presencaService.CreateBatch(presencaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Presenças criadas com sucesso!",
                    Success = true,
                    Data = presencasCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] PresencaDTO presencaDTO)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(presencaDTO.PoloId);

                var presencaUpdated = await _presencaService.Update(presencaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Presença atualizada com sucesso!",
                    Success = true,
                    Data = presencaUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var presenca = await _presencaService.Get(id);

                if (presenca == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Presença foi encontrada com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _presencaService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Presença removida com sucesso!",
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
                var presenca = await _presencaService.Get(id);

                if (presenca == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Presença foi encontrada com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Presença encontrada com sucesso!",
                    Success = true,
                    Data = presenca
                });
            });
        }

        [HttpGet("aula/{aulaId}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> GetPresencasPorAula(long aulaId)
        {
            return await ExecuteAsync(async () =>
            {
                var presencas = await _presencaService.GetPresencasPorAula(aulaId);

                if (presencas == null || !presencas.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma presença encontrada para a aula informada!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Presenças encontradas com sucesso!",
                    Success = true,
                    Data = presencas
                });
            });
        }
    }
}
