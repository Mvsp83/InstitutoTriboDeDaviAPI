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
    public class PresencaController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPresencaService _presencaService;

        public PresencaController(IPresencaService presencaService, IMapper mapper)
        {
            _mapper = mapper;
            _presencaService = presencaService;
        }

        [HttpGet]
        [Route("/presenca/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allPresencas = await _presencaService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Presenças encontrados com sucesso!",
                    Success = true,
                    Data = allPresencas
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
        [Route("/presenca/create")]
        public async Task<IActionResult> Create([FromBody] PresencaViewModel presencaViewModel)
        {
            try
            {
                var presencaDTO = _mapper.Map<PresencaDTO>(presencaViewModel);
                var presencaCreated = await _presencaService.Create(presencaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Presença criada com sucesso!",
                    Success = true,
                    Data = presencaCreated
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
        [Route("/presenca/batch/create")]
        public async Task<IActionResult> CreateBatch([FromBody] List<PresencaViewModel> presencaViewModels)
        {
            try
            {
                var presencaDTOs = _mapper.Map<List<PresencaDTO>>(presencaViewModels);
                var presencasCreated = await _presencaService.CreateBatch(presencaDTOs);

                return Ok(new ResultViewModel
                {
                    Message = "Presenças criadas com sucesso!",
                    Success = true,
                    Data = presencasCreated
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
        [Route("/presenca/update")]
        public async Task<IActionResult> Update([FromBody] PresencaDTO presencaDTO)
        {
            try
            {
                var presencaUpdated = await _presencaService.Update(presencaDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Presença atualizada com sucesso!",
                    Success = true,
                    Data = presencaUpdated
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
        [Route("/presenca/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var presenca = await _presencaService.Get(id);

                if (presenca == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Presença foi encontrada com o ID informado!",
                        Success = true,
                        Data = presenca
                    });
                }

                await _presencaService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Presença removida com sucesso!",
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
        [Route("/presenca/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var presenca = await _presencaService.Get(id);

                if (presenca == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Presença foi encontrada com o ID informado!",
                        Success = true,
                        Data = presenca
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Presença encontrada com sucesso!",
                    Success = true,
                    Data = presenca
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
        [Route("/presenca/aula/{aulaId}")]
        public async Task<IActionResult> GetPresencasPorAula(long aulaId)
        {
            try
            {
                var presencas = await _presencaService.GetPresencasPorAula(aulaId);

                if (presencas == null || !presencas.Any())
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma presença encontrada para a aula informada!",
                        Success = true,
                        Data = presencas
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Presenças encontradas com sucesso!",
                    Success = true,
                    Data = presencas
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
