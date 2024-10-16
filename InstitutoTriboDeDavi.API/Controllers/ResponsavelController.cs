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
    public class ResponsavelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IResponsavelService _responsavelService;

        public ResponsavelController(IMapper mapper, IResponsavelService responsavelService)
        {
            _mapper = mapper;
            _responsavelService = responsavelService;
        }

        [HttpGet]
        [Route("/responsavel/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allResponsaveis = await _responsavelService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Responsáveis encontrados com sucesso!",
                    Success = true,
                    Data = allResponsaveis
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
        [Route("/responsavel/create")]
        public async Task<IActionResult> Create([FromBody] ResponsavelViewModel responsavelViewModel)
        {
            try
            {
                var responsavelDTO = _mapper.Map<ResponsavelDTO>(responsavelViewModel);
                var responsavelCreated = await _responsavelService.Create(responsavelDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Responsável criado com sucesso!",
                    Success = true,
                    Data = responsavelCreated
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
        [Route("/responsavel/update")]
        public async Task<IActionResult> Update([FromBody] ResponsavelDTO responsavelDTO)
        {
            try
            {
                var responsavelUpdated = await _responsavelService.Update(responsavelDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Responsável atualizado com sucesso!",
                    Success = true,
                    Data = responsavelUpdated
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
        [Route("/responsavel/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var responsavel = await _responsavelService.Get(id);

                if (responsavel == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Responsável foi encontrado com o ID informado!",
                        Success = true,
                        Data = responsavel
                    });
                }

                await _responsavelService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Responsável removido com sucesso!",
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
        [Route("/responsavel/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var responsavel = await _responsavelService.Get(id);

                if (responsavel == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Responsável foi encontrado com o ID informado!",
                        Success = true,
                        Data = responsavel
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Responsável encontrado com sucesso!",
                    Success = true,
                    Data = responsavel
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
        [Route("/responsavel/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var responsavel = await _responsavelService.GetByNome(nome);

                if (responsavel == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Responsável foi encontrado com o Nome informado!",
                        Success = true,
                        Data = responsavel
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Responsável encontrado com sucesso!",
                    Success = true,
                    Data = responsavel
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
        [Route("/responsavel/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allResponsaveis = await _responsavelService.SearchByNome(nome);

                if (allResponsaveis.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Responsável foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Responsáveis encontrados com sucesso!",
                    Success = true,
                    Data = allResponsaveis
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
