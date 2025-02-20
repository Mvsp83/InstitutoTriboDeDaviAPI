using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.DTO.Business;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AulaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAulaService _aulaService;

        public AulaController(IAulaService aulaService, IMapper mapper)
        {
            _mapper = mapper;
            _aulaService = aulaService;
        }

        [HttpGet]
        [Authorize]
        [Route("/aula/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allAulas = await _aulaService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Aulas encontradas com sucesso!",
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
        [Authorize]
        [Route("/aula/create")]
        public async Task<IActionResult> Create([FromBody] AulaDTO aulaDTO)
        {
            try
            {
                if (UsuarioAutenticado.PoloId != aulaDTO.PoloId)
                {
                    throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
                }

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
        [Authorize]
        [Route("/aula/update")]
        public async Task<IActionResult> Update([FromBody] AulaDTO aulaDTO)
        {
            try
            {
                if (UsuarioAutenticado.PoloId != aulaDTO.PoloId)
                {
                    throw new UnauthorizedAccessException("Usuário não tem permissão.");
                }

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
        [Authorize]
        [Route("/aula/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (UsuarioAutenticado.Role != UserRole.Administrador)
                {
                    throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
                }

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
        [Authorize]
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

        [HttpGet]
        [Authorize]
        [Route("/aula/get-por-polo")]
        public async Task<IActionResult> ObterAulas([FromQuery] IEnumerable<int> turmas)
        {
            try
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
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno: " + ex.Message);
            }
        }
    }
}

