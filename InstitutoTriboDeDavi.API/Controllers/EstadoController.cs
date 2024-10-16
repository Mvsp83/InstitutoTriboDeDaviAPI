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
    public class EstadoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEstadoService _estadoService;

        public EstadoController(IMapper mapper, IEstadoService estadoService)
        {
            _mapper = mapper;
            _estadoService = estadoService;
        }

        [HttpGet]
        [Route("/estado/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allEstados = await _estadoService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Estados encontrados com sucesso!",
                    Success = true,
                    Data = allEstados
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
        [Route("/estado/create")]
        public async Task<IActionResult> Create([FromBody] EstadoViewModel estadoViewModel)
        {
            try
            {
                var estadoDTO = _mapper.Map<EstadoDTO>(estadoViewModel);
                var estadoCreated = await _estadoService.Create(estadoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Estado criado com sucesso!",
                    Success = true,
                    Data = estadoCreated
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
        [Route("/estado/update")]
        public async Task<IActionResult> Update([FromBody] EstadoDTO estadoDTO)
        {
            try
            {
                var estadoUpdated = await _estadoService.Update(estadoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Estado atualizado com sucesso!",
                    Success = true,
                    Data = estadoUpdated
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
        [Route("/estado/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var estado = await _estadoService.Get(id);

                if (estado == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Estado foi encontrado com o ID informado!",
                        Success = true,
                        Data = estado
                    });
                }

                await _estadoService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Estado removido com sucesso!",
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
        [Route("/estado/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var estado = await _estadoService.Get(id);

                if (estado == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Estado foi encontrado com o ID informado!",
                        Success = true,
                        Data = estado
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Estado encontrado com sucesso!",
                    Success = true,
                    Data = estado
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
        [Route("/estado/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var estado = await _estadoService.GetByNome(nome);

                if (estado == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Estado foi encontrado com o Nome informado!",
                        Success = true,
                        Data = estado
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Estado encontrado com sucesso!",
                    Success = true,
                    Data = estado
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
        [Route("/estado/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allEstados = await _estadoService.SearchByNome(nome);

                if (allEstados.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Estado foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Estados encontrados com sucesso!",
                    Success = true,
                    Data = allEstados
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
