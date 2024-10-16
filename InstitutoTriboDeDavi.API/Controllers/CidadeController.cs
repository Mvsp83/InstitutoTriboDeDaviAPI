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
    public class CidadeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICidadeService _cidadeService;

        public CidadeController(IMapper mapper, ICidadeService cidadeService)
        {
            _mapper = mapper;
            _cidadeService = cidadeService;
        }

        [HttpGet]
        [Route("/cidade/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allCidades = await _cidadeService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Cidades encontradas com sucesso!",
                    Success = true,
                    Data = allCidades
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
        [Route("/cidade/create")]
        public async Task<IActionResult> Create([FromBody] CidadeViewModel cidadeViewModel)
        {
            try
            {
                var cidadeDTO = _mapper.Map<CidadeDTO>(cidadeViewModel);
                var cidadeCreated = await _cidadeService.Create(cidadeDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Cidade criada com sucesso!",
                    Success = true,
                    Data = cidadeCreated
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
        [Route("/cidade/update")]
        public async Task<IActionResult> Update([FromBody] CidadeDTO cidadeDTO)
        {
            try
            {
                var cidadeUpdated = await _cidadeService.Update(cidadeDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Cidade atualizada com sucesso!",
                    Success = true,
                    Data = cidadeUpdated
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
        [Route("/cidade/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var cidade = await _cidadeService.Get(id);

                if (cidade == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Cidade foi encontrado com o ID informado!",
                        Success = true,
                        Data = cidade
                    });
                }

                await _cidadeService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Cidade removida com sucesso!",
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
        [Route("/cidade/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var cidade = await _cidadeService.Get(id);

                if (cidade == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Cidade foi encontrada com o ID informado!",
                        Success = true,
                        Data = cidade
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Cidade encontrada com sucesso!",
                    Success = true,
                    Data = cidade
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
        [Route("/cidade/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var cidade = await _cidadeService.GetByNome(nome);

                if (cidade == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Cidade foi encontrada com o Nome informado!",
                        Success = true,
                        Data = cidade
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Cidade encontrado com sucesso!",
                    Success = true,
                    Data = cidade
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
        [Route("/cidade/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allCidades = await _cidadeService.SearchByNome(nome);

                if (allCidades.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Cidade foi encontrada com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Cidades encontradas com sucesso!",
                    Success = true,
                    Data = allCidades
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
