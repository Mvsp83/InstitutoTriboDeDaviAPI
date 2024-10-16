using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class PoloController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPoloService _poloService;

        public PoloController(IMapper mapper, IPoloService poloService)
        {
            _mapper = mapper;
            _poloService = poloService;
        }

        [HttpGet]
        [Route("/polo/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allPolos = await _poloService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Polos encontrados com sucesso!",
                    Success = true,
                    Data = allPolos
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
        [Route("/polo/create")]
        public async Task<IActionResult> Create([FromBody] PoloViewModel poloViewModel)
        {
            try
            {
                var poloDTO = _mapper.Map<PoloDTO>(poloViewModel);
                var poloCreated = await _poloService.Create(poloDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Polo criado com sucesso!",
                    Success = true,
                    Data = poloCreated
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
        [Route("/polo/update")]
        public async Task<IActionResult> Update([FromBody] PoloDTO poloDTO)
        {
            try
            {
                var poloUpdated = await _poloService.Update(poloDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Polo atualizado com sucesso!",
                    Success = true,
                    Data = poloUpdated
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
        [Route("/polo/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var polo = await _poloService.Get(id);

                if (polo == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Polo foi encontrado com o ID informado!",
                        Success = true,
                        Data = polo
                    });
                }

                await _poloService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Polo removido com sucesso!",
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
        [Route("/polo/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var polo = await _poloService.Get(id);

                if (polo == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Polo foi encontrado com o ID informado!",
                        Success = true,
                        Data = polo
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Polo encontrado com sucesso!",
                    Success = true,
                    Data = polo
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
        [Route("/polo/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var polo = await _poloService.GetByNome(nome);

                if (polo == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Polo foi encontrado com o Nome informado!",
                        Success = true,
                        Data = polo
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Polo encontrado com sucesso!",
                    Success = true,
                    Data = polo
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
        [Route("/polo/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allPolos = await _poloService.SearchByNome(nome);

                if (allPolos.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Polo foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Polos encontrados com sucesso!",
                    Success = true,
                    Data = allPolos
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
