using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Business;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.DTO.Business;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AulaController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAulaService _aulaService;

        public AulaController(IAulaService aulaService, IMapper mapper)
        {
            _mapper = mapper;
            _aulaService = aulaService;
        }

        [HttpGet]
        [Route("/aula/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allAulas = await _aulaService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Aulas encontrados com sucesso!",
                    Success = true,
                    Data = allAulas
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpPost]
        [Route("/aula/create")]
        public async Task<IActionResult> Create([FromBody] AulaViewModel aulaViewModel)
        {
            try
            {
                var aulaDTO = _mapper.Map<AulaDTO>(aulaViewModel);
                var aulaCreated = await _aulaService.Create(aulaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aula criada com sucesso!",
                    Success = true,
                    Data = aulaCreated
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpPut]
        [Route("/aula/update")]
        public async Task<IActionResult> Update([FromBody] AulaDTO aulaDTO)
        {
            try
            {
                var aulaUpdated = await _aulaService.Update(aulaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aula atualizada com sucesso!",
                    Success = true,
                    Data = aulaUpdated
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpDelete]
        [Route("/aula/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
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

                await _aulaService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Aula removida com sucesso!",
                    Success = true,
                    Data = null
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpGet]
        [Route("/aula/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
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
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }
    }
}

