using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Models;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AlunoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAlunoService _alunoService;

        public AlunoController(IMapper mapper, IAlunoService alunoService)
        {
            _mapper = mapper;
            _alunoService = alunoService;
        }

        [HttpGet]
        [Route("/aluno/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allAlunos = await _alunoService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Alunos encontrados com sucesso!",
                    Success = true,
                    Data = allAlunos
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
        [Route("/aluno/create")]
        public async Task<IActionResult> Create([FromBody] AlunoViewModel alunoViewModel)
        {
            try
            {
                var alunoDTO = _mapper.Map<AlunoDTO>(alunoViewModel);
                var alunoCreated = await _alunoService.Create(alunoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno criado com sucesso!",
                    Success = true,
                    Data = alunoCreated
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
        [Route("/aluno/update")]
        public async Task<IActionResult> Update([FromBody] AlunoDTO alunoDTO)
        {
            try
            {
                var alunoUpdated = await _alunoService.Update(alunoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno atualizado com sucesso!",
                    Success = true,
                    Data = alunoUpdated
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
        [Route("/aluno/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var aluno = await _alunoService.Get(id);

                if (aluno == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = aluno
                    });
                }

                await _alunoService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno removido com sucesso!",
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
        [Route("/aluno/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var aluno = await _alunoService.Get(id);

                if (aluno == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = aluno
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Aluno encontrado com sucesso!",
                    Success = true,
                    Data = aluno
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
        [Route("/aluno/get-by-nome")]
        public async Task<IActionResult> GetByNome([FromQuery] string nome)
        {
            try
            {
                var aluno = await _alunoService.GetByNome(nome);

                if (aluno == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado com o Nome informado!",
                        Success = true,
                        Data = aluno
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Aluno encontrado com sucesso!",
                    Success = true,
                    Data = aluno
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
        [Route("/aluno/search-by-nome")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string nome)
        {
            try
            {
                var allAlunos = await _alunoService.SearchByNome(nome);

                if (allAlunos.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado com o Nome informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Alunos encontrados com sucesso!",
                    Success = true,
                    Data = allAlunos
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
