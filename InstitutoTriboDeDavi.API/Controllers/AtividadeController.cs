using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtividadeController : BaseController
    {
        private readonly IAtividadeService _atividadeService;
        private readonly ILogger<AtividadeController> _logger;

        public AtividadeController(IAtividadeService atividadeService, ILogger<AtividadeController> logger) : base(logger)
        {
            _atividadeService = atividadeService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allAtividades = await _atividadeService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Atividades encontradas com sucesso!",
                    Success = true,
                    Data = allAtividades
                });
            });
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var atividade = await _atividadeService.Get(id);

                if (atividade == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Atividade foi encontrada com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Atividade encontrada com sucesso!",
                    Success = true,
                    Data = atividade
                });
            });
        }

        [HttpGet("historico-turma")]
        [Authorize]
        public async Task<IActionResult> HistoricoTurma([FromQuery] long poloId, [FromQuery] int turma)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(poloId);

                var historico = await _atividadeService.ObterHistoricoTurmaAsync(poloId, turma);

                return Ok(new ResultViewModel
                {
                    Message = "Histórico obtido com sucesso!",
                    Success = true,
                    Data = historico
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Create([FromBody] AtividadeDTO atividadeDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var atividadeCreated = await _atividadeService.Create(atividadeDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Atividade criada com sucesso!",
                    Success = true,
                    Data = atividadeCreated
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] AtividadeDTO atividadeDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var atividadeUpdated = await _atividadeService.Update(atividadeDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Atividade atualizada com sucesso!",
                    Success = true,
                    Data = atividadeUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var atividade = await _atividadeService.Get(id);

                if (atividade == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma Atividade foi encontrada com o ID informado!",
                        Success = true,
                        Data = null
                    });

                await _atividadeService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Atividade removida com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
