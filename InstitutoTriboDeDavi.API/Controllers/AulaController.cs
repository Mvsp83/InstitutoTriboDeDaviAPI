using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Business;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;
using Microsoft.AspNet.Identity;
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

        public AulaController(IAulaService aulaService, IMapper mapper, ILogger<AulaController> logger) : base(logger as ILogger<Controller>)
        {
            _mapper = mapper;
            _aulaService = aulaService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        [Route("get-all")]
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

        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] AulaDTO aulaDTO)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(UsuarioAutenticado, aulaDTO.PoloId);
                ValidateUserRole(UserRole.Professor);

                var aulaCreated = await _aulaService.Create(aulaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aula criada com sucesso!",
                    Success = true,
                    Data = aulaCreated
                });
            });
        }

        [HttpPut]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] AulaDTO aulaDTO)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(UsuarioAutenticado, aulaDTO.PoloId);

                var aulaUpdated = await _aulaService.Update(aulaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aula atualizada com sucesso!",
                    Success = true,
                    Data = aulaUpdated
                });
            });
        }

        [HttpDelete]
        [Authorize]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                ValidateUserRole(UserRole.Administrador);

                var aula = await _aulaService.Get(id);

                if (aula == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Aula foi encontrada com o ID informado!",
                        Success = true,
                        Data = aula
                    });
                }

                await _aulaService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Aula removida com sucesso!",
                    Success = true,
                    Data = null
                });
            });           
        }

        [HttpGet]
        [Authorize]
        [Route("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var aula = await _aulaService.Get(id);

                if (aula == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Aula foi encontrada com o ID informado!",
                        Success = true,
                        Data = aula
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Aula encontrada com sucesso!",
                    Success = true,
                    Data = aula
                });
            });
        }

        [HttpGet]
        [Authorize]
        [Route("get-por-polo")]
        public async Task<IActionResult> ObterAulas([FromQuery] IEnumerable<int> turmas)
        {
            return await ExecuteAsync(async () =>
            {
                var allAulas = await _aulaService.ObterAulasTurmaAsync(UsuarioAutenticado, turmas);
                if (allAulas.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aula foi encontrado!",
                        Success = true,
                        Data = null
                    });
                }
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

