using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Queries;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AlunoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAlunoService _alunoService;
        private readonly IPresencaService _presencaService;
        private readonly IFrequenciaService _frequenciaService;

        public AlunoController(IMapper mapper, IAlunoService alunoService, IPresencaService presencaService, IFrequenciaService frequenciaService)
        {
            _mapper = mapper;
            _alunoService = alunoService;
            _presencaService = presencaService;
            _frequenciaService = frequenciaService;
        }

        [HttpGet]
        [Route("/aluno/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allAlunos = await _alunoService.GetAll();

                var alunosComDescricao = allAlunos.Select(static aluno => new
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    RG = aluno.RG,
                    CPF = aluno.CPF,
                    DataNascimento = aluno.DataNascimento,
                    Peso = aluno.Peso.ToString(),
                    Faixa = aluno.Faixa,
                    Endereco = aluno.Endereco,
                    Bairro = aluno.Bairro,
                    Cidade = aluno.Cidade,
                    Celular = aluno.Celular,
                    Responsavel = aluno.Responsavel,
                    Parentesco = aluno.Parentesco,
                    RGResponsavel = aluno.RGResponsavel,
                    CPFResponsavel = aluno.CPFResponsavel,
                    Escola = aluno.Escola,
                    Periodo = aluno.Periodo,
                    PoloId = aluno.PoloId
                });

                return Ok(new ResultViewModel
                {
                    Message = "Alunos encontrados com sucesso!",
                    Success = true,
                    Data = alunosComDescricao
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

                var alunosComDescricao = allAlunos.Select(static aluno => new
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    DataNascimento = aluno.DataNascimento,
                    Faixa = aluno.Faixa.GetDescription(),
                });

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
                    Data = alunosComDescricao
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
        [Route("/aluno/total")]
        public async Task<IActionResult> GetTotalAlunos()
        {
            try
            {
                var totalAlunos = await _alunoService.GetTotalAlunos();
                return Ok(new ResultViewModel
                {
                    Message = "Total de alunos obtido com sucesso!",
                    Success = true,
                    Data = totalAlunos
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
        [Route("/aluno/alunos-mais-faltantes")]
        public async Task<IActionResult> GetAlunosFaltas(long poloId)
        {
            try
            {
                List<FrequenciaDTO> totalAlunos = await _frequenciaService.GetAlunosFaltas(poloId);

                var totalAlunosComDescricao = totalAlunos.Select(static aluno => new
                {
                    AlunoId = aluno.AlunoId,
                    Nome = aluno.Nome,
                    Faixa = aluno.Faixa.GetDescription(), 
                    TotalAulas = aluno.TotalAulas,
                    TotalFaltas = aluno.TotalFaltas
                });

                if (totalAlunosComDescricao.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Faltas de alunos obtidas com sucesso!",
                    Success = true,
                    Data = totalAlunosComDescricao
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
