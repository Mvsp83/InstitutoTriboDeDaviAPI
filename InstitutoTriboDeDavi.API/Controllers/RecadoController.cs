using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Mural de recados (classificados da comunidade). Leitura para qualquer pessoa
    // logada — inclui o portal do responsável; criar/editar/remover é da equipe.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RequireModulo("relacionamento")]
    public class RecadoController : BaseController
    {
        private readonly IRecadoService _service;
        private readonly ILogger<RecadoController> _logger;

        public RecadoController(IRecadoService service, ILogger<RecadoController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // Feed do mural: recados vigentes, visível a quem está logado (equipe e
        // portal do responsável). Roles explícitos para o token do responsável
        // também passar (e para não depender só do [Authorize] genérico).
        [HttpGet("mural")]
        [Authorize(Roles = "Administrador,Supervisor,Professor,Responsavel")]
        public async Task<IActionResult> Mural()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Mural obtido com sucesso!",
                Success = true,
                Data = await _service.ListarVigentes()
            }));
        }

        // Gestão da equipe: todos os recados (inclui expirados/inativos).
        [HttpGet("gerenciar")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Gerenciar()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Recados obtidos com sucesso!",
                Success = true,
                Data = await _service.ListarTodos()
            }));
        }

        [HttpPost("create")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Create([FromBody] RecadoDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Recado publicado com sucesso!",
                Success = true,
                Data = await _service.Create(dto, UsuarioAutenticado)
            }));
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] RecadoDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Recado atualizado com sucesso!",
                Success = true,
                Data = await _service.Update(dto, UsuarioAutenticado)
            }));
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Delete(id, UsuarioAutenticado);
                return Ok(new ResultViewModel
                {
                    Message = "Recado removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
