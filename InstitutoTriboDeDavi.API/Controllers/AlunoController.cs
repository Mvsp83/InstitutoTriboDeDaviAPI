using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : BaseController
    {
        private readonly IAlunoService _alunoService;
        private readonly IFrequenciaService _frequenciaService;
        private readonly ILogger<AlunoController> _logger;

        public AlunoController(IAlunoService alunoService, IPresencaService presencaService, IFrequenciaService frequenciaService, ILogger<AlunoController> logger) : base(logger)
        {
            _alunoService = alunoService;
            _frequenciaService = frequenciaService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allAlunos = await _alunoService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Alunos encontrados com sucesso!",
                    Success = true,
                    Data = allAlunos
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Create([FromBody] AlunoDTO alunoDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var alunoCreated = await _alunoService.Create(alunoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno criado com sucesso!",
                    Success = true,
                    Data = alunoCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] AlunoDTO alunoDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var alunoUpdated = await _alunoService.Update(alunoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno atualizado com sucesso!",
                    Success = true,
                    Data = alunoUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var aluno = await _alunoService.Get(id);

                if (aluno == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _alunoService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Aluno removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpGet("alunos-mais-faltantes")]
        [Authorize]
        public async Task<IActionResult> GetAlunosFaltas()
        {
            return await ExecuteAsync(async () =>
            {
                var totalAlunos = await _frequenciaService.GetAlunosFaltasAsync(UsuarioAutenticado);

                if (!totalAlunos.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Alunos obtidos com sucesso!",
                    Success = true,
                    Data = totalAlunos
                });
            });
        }

        [HttpGet("get-por-polo")]
        [Authorize]
        public async Task<IActionResult> ObterAlunos([FromQuery] List<int> turmas)
        {
            return await ExecuteAsync(async () =>
            {
                var allAlunos = await _alunoService.ObterAlunosPorTurmaAsync(UsuarioAutenticado, turmas);

                if (!allAlunos.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Alunos obtidos com sucesso!",
                    Success = true,
                    Data = allAlunos
                });
            });
        }

        [HttpGet("pendentes")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> ObterPendentes()
        {
            return await ExecuteAsync(async () =>
            {
                var pendentes = await _alunoService.ObterAlunosPendentesAsync(UsuarioAutenticado);

                if (!pendentes.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum aluno pendente de turma encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = $"{pendentes.Count} aluno(s) aguardando atribuição de turma.",
                    Success = true,
                    Data = pendentes
                });
            });
        }

        [HttpPatch("atribuir-turma")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> AtribuirTurma([FromBody] AtribuirTurmaDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var aluno = await _alunoService.AtribuirTurmaAsync(dto);

                return Ok(new ResultViewModel
                {
                    Message = $"Turma {dto.Turma} atribuída ao aluno '{aluno.Nome}' com sucesso!",
                    Success = true,
                    Data = aluno
                });
            });
        }
    }
}
