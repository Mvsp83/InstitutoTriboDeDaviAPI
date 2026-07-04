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
    public class AulaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAulaService _aulaService;
        private readonly ILogger<AulaController> _logger;

        public AulaController(IAulaService aulaService, IMapper mapper, ILogger<AulaController> logger) : base(logger)
        {
            _mapper = mapper;
            _aulaService = aulaService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allAulas = await _aulaService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Aulas encontradas com sucesso!",
                    Success = true,
                    Data = allAulas
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Create([FromBody] AulaDTO aulaDTO)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(aulaDTO.PoloId);

                var aulaCreated = await _aulaService.Create(aulaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aula criada com sucesso!",
                    Success = true,
                    Data = aulaCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] AulaDTO aulaDTO)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(aulaDTO.PoloId);

                var aulaUpdated = await _aulaService.Update(aulaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aula atualizada com sucesso!",
                    Success = true,
                    Data = aulaUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var aula = await _aulaService.Get(id);

                if (aula == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Aula foi encontrada com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _aulaService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Aula removida com sucesso!",
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
                var aula = await _aulaService.Get(id);

                if (aula == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Aula foi encontrada com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Aula encontrada com sucesso!",
                    Success = true,
                    Data = aula
                });
            });
        }

        [HttpGet("get-por-polo")]
        [Authorize]
        public async Task<IActionResult> ObterAulas([FromQuery] IEnumerable<int> turmas)
        {
            return await ExecuteAsync(async () =>
            {
                var allAulas = await _aulaService.ObterAulasTurmaAsync(UsuarioAutenticado, turmas);

                if (!allAulas.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Aula foi encontrada!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Aulas obtidas com sucesso!",
                    Success = true,
                    Data = allAulas
                });
            });
        }
    }
}
