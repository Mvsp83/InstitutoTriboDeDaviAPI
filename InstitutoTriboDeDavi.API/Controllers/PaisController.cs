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
    public class PaisController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPaisService _paisService;

        public PaisController(IMapper mapper, IPaisService paisService)
        {
            _mapper = mapper;
            _paisService = paisService;
        }

        [HttpGet]
        [Route("/pais/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allPaises = await _paisService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Paises encontrados com sucesso!",
                    Success = true,
                    Data = allPaises
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
        [Route("/pais/create")]
        public async Task<IActionResult> Create([FromBody] PaisViewModel paisViewModel)
        {
            try
            {
                var paisDTO = _mapper.Map<PaisDTO>(paisViewModel);
                var paisCreated = await _paisService.Create(paisDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Pais criado com sucesso!",
                    Success = true,
                    Data = paisCreated
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
        [Route("/pais/update")]
        public async Task<IActionResult> Update([FromBody] PaisDTO paisDTO)
        {
            try
            {
                var paisUpdated = await _paisService.Update(paisDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Pais atualizado com sucesso!",
                    Success = true,
                    Data = paisUpdated
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
        [Route("/pais/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var pais = await _paisService.Get(id);

                if (pais == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Pais foi encontrado com o ID informado!",
                        Success = true,
                        Data = pais
                    });
                }

                await _paisService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Pais removido com sucesso!",
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
        [Route("/pais/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var pais = await _paisService.Get(id);

                if (pais == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Pais foi encontrado com o ID informado!",
                        Success = true,
                        Data = pais
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Pais encontrado com sucesso!",
                    Success = true,
                    Data = pais
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
        [Route("/pais/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var pais = await _paisService.GetByNome(nome);

                if (pais == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Pais foi encontrado com o Nome informado!",
                        Success = true,
                        Data = pais
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Pais encontrado com sucesso!",
                    Success = true,
                    Data = pais
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
        [Route("/pais/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allPaises = await _paisService.SearchByNome(nome);

                if (allPaises.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Pais foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Paises encontrados com sucesso!",
                    Success = true,
                    Data = allPaises
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
