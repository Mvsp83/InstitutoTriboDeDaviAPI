using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Advertências e recados do professor sobre um aluno (aparecem no portal do
    // responsável). Só professor/supervisor/admin registram.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
    public class OcorrenciaController : BaseController
    {
        private readonly IOcorrenciaAlunoService _service;
        private readonly ILogger<OcorrenciaController> _logger;

        public OcorrenciaController(IOcorrenciaAlunoService service, ILogger<OcorrenciaController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("aluno/{alunoId}")]
        public async Task<IActionResult> PorAluno(long alunoId)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Ocorrências obtidas.",
                Success = true,
                Data = await _service.ListarPorAluno(alunoId)
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] OcorrenciaAlunoDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var criado = await _service.Criar(dto, UsuarioAutenticado.Login);
                _logger.LogInformation(
                    "Ocorrência (tipo {Tipo}) registrada para o aluno #{Id} por {Login}.",
                    dto.Tipo, dto.AlunoId, UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Registro salvo.",
                    Success = true,
                    Data = criado
                });
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Excluir(id);
                return Ok(new ResultViewModel { Message = "Registro removido.", Success = true, Data = null });
            });
        }
    }
}
