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
    public class EnderecoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnderecoService _enderecoService;

        public EnderecoController(IMapper mapper, IEnderecoService enderecoService)
        {
            _mapper = mapper;
            _enderecoService = enderecoService;
        }

        [HttpGet]
        [Route("/endereco/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allEnderecos = await _enderecoService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Endereços encontrados com sucesso!",
                    Success = true,
                    Data = allEnderecos
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
        [Route("/endereco/create")]
        public async Task<IActionResult> Create([FromBody] EnderecoViewModel enderecoViewModel)
        {
            try
            {
                var enderecoDTO = _mapper.Map<EnderecoDTO>(enderecoViewModel);
                var enderecoCreated = await _enderecoService.Create(enderecoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Endereco criado com sucesso!",
                    Success = true,
                    Data = enderecoCreated
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
        [Route("/endereco/update")]
        public async Task<IActionResult> Update([FromBody] EnderecoDTO enderecoDTO)
        {
            try
            {
                var enderecoUpdated = await _enderecoService.Update(enderecoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Endereço atualizado com sucesso!",
                    Success = true,
                    Data = enderecoUpdated
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
        [Route("/endereco/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var endereco = await _enderecoService.Get(id);

                if (endereco == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Endereço foi encontrado com o ID informado!",
                        Success = true,
                        Data = endereco
                    });
                }

                await _enderecoService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Endereço removido com sucesso!",
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
        [Route("/endereco/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var endereco = await _enderecoService.Get(id);

                if (endereco == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Endereço foi encontrado com o ID informado!",
                        Success = true,
                        Data = endereco
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Endereço encontrado com sucesso!",
                    Success = true,
                    Data = endereco
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
        [Route("/endereco/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var endereco = await _enderecoService.GetByNome(nome);

                if (endereco == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Endereço foi encontrado com o Nome informado!",
                        Success = true,
                        Data = endereco
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Endereço encontrado com sucesso!",
                    Success = true,
                    Data = endereco
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
        [Route("/endereco/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allEnderecos = await _enderecoService.SearchByNome(nome);

                if (allEnderecos.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Endereço foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Endereços encontrados com sucesso!",
                    Success = true,
                    Data = allEnderecos
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
