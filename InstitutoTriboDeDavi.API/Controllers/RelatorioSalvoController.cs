using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioSalvoController : BaseController
    {
        private readonly IRelatorioSalvoService _relatorioService;
        private readonly ILogger<RelatorioSalvoController> _logger;

        public RelatorioSalvoController(IRelatorioSalvoService relatorioService, ILogger<RelatorioSalvoController> logger) : base(logger)
        {
            _relatorioService = relatorioService;
            _logger = logger;
        }

        [HttpGet("get-meus")]
        [Authorize]
        public async Task<IActionResult> GetMeus()
        {
            return await ExecuteAsync(async () =>
            {
                var relatorios = await _relatorioService.GetPorUsuario(UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Relatórios encontrados com sucesso!",
                    Success = true,
                    Data = relatorios
                });
            });
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RelatorioSalvoDTO relatorioDTO)
        {
            return await ExecuteAsync(async () =>
            {
                // O dono é sempre o usuário autenticado, ignorando o que vier no body
                relatorioDTO.Id = 0;
                relatorioDTO.UsuarioLogin = UsuarioAutenticado.Login;

                var relatorioCreated = await _relatorioService.Create(relatorioDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Relatório salvo com sucesso!",
                    Success = true,
                    Data = relatorioCreated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _relatorioService.Delete(id, UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = "Relatório removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
