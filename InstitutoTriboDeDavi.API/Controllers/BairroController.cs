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
    public class BairroController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBairroService _bairroService;

        public BairroController(IMapper mapper, IBairroService bairroService)
        {
            _mapper = mapper;
            _bairroService = bairroService;
        }

        [HttpGet]
        [Route("/bairro/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allBairros = await _bairroService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Bairros encontrados com sucesso!",
                    Success = true,
                    Data = allBairros
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
        [Route("/bairro/create")]
        public async Task<IActionResult> Create([FromBody] BairroViewModel bairroViewModel)
        {
            try
            {
                var bairroDTO = _mapper.Map<BairroDTO>(bairroViewModel);
                var bairroCreated = await _bairroService.Create(bairroDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Bairro criado com sucesso!",
                    Success = true,
                    Data = bairroCreated
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
        [Route("/bairro/update")]
        public async Task<IActionResult> Update([FromBody] BairroDTO bairroDTO)
        {
            try
            {
                var bairroUpdated = await _bairroService.Update(bairroDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Bairro atualizado com sucesso!",
                    Success = true,
                    Data = bairroUpdated
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
        [Route("/bairro/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var bairro = await _bairroService.Get(id);

                if (bairro == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Bairro foi encontrado com o ID informado!",
                        Success = true,
                        Data = bairro
                    });
                }

                await _bairroService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Bairro removido com sucesso!",
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
        [Route("/bairro/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var bairro = await _bairroService.Get(id);

                if (bairro == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Bairro foi encontrado com o ID informado!",
                        Success = true,
                        Data = bairro
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Bairro encontrado com sucesso!",
                    Success = true,
                    Data = bairro
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
        [Route("/bairro/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var bairro = await _bairroService.GetByNome(nome);

                if (bairro == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Bairro foi encontrado com o Nome informado!",
                        Success = true,
                        Data = bairro
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Bairro encontrado com sucesso!",
                    Success = true,
                    Data = bairro
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
        [Route("/bairro/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allBairros = await _bairroService.SearchByNome(nome);

                if (allBairros.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Bairro foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Bairros encontrados com sucesso!",
                    Success = true,
                    Data = allBairros
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
