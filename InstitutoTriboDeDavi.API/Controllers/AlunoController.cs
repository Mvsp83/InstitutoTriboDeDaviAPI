using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Queries;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AlunoController : BaseController
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
        [Authorize]
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
                    Peso = aluno.Peso,
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
                    PoloId = aluno.PoloId,
                    Turma = aluno.Turma
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
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/aluno/create")]
        public async Task<IActionResult> Create([FromBody] AlunoDTO alunoDTO)
        {
            try
            {
                if (UsuarioAutenticado.Role != UserRole.Administrador)
                {
                    throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
                }

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
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpPut]
        [Authorize]
        [Route("/aluno/update")]
        public async Task<IActionResult> Update([FromBody] AlunoDTO alunoDTO)
        {
            try
            {
                if (UsuarioAutenticado.Role != UserRole.Professor)
                {
                    throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
                }

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
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/aluno/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (UsuarioAutenticado.Role != UserRole.Administrador)
                {
                    throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
                }

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
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/aluno/alunos-mais-faltantes")]
        public async Task<IActionResult> GetAlunosFaltas()
        {
            try
            {
                List<FrequenciaDTO> totalAlunos = await _frequenciaService.GetAlunosFaltasAsync(UsuarioAutenticado);

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
                    Message = "Alunos obtidos com sucesso!",
                    Success = true,
                    Data = totalAlunosComDescricao
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/aluno/get-por-polo")]
        public async Task<IActionResult> ObterAlunos([FromQuery] List<int> turmas)
        {
            try
            {
                var allAlunos = await _alunoService.ObterAlunosPorTurmaAsync(UsuarioAutenticado, turmas);

                var alunosComDescricao = allAlunos.Select(static aluno => new
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    RG = aluno.RG,
                    CPF = aluno.CPF,
                    DataNascimento = aluno.DataNascimento,
                    Peso = aluno.Peso,
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
                    PoloId = aluno.PoloId,
                    Turma = aluno.Turma
                });


                if (allAlunos.Count() == 0)
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
                    Message = "Alunos obtidos com sucesso!",
                    Success = true,
                    Data = alunosComDescricao
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno: " + ex.Message);
            }
        }
    }
}
