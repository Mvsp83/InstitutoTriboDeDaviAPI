using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracaoDashboardController : BaseController
    {
        private readonly IConfiguracaoDashboardService _service;
        private readonly ILogger<ConfiguracaoDashboardController> _logger;

        public ConfiguracaoDashboardController(
            IConfiguracaoDashboardService service,
            ILogger<ConfiguracaoDashboardController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // Cada usuário lê a própria preferência de layout do painel.
        [HttpGet("obter")]
        [Authorize]
        public async Task<IActionResult> Obter()
        {
            return await ExecuteAsync(async () =>
            {
                var configuracao = await _service.Obter(UsuarioAutenticado.Login);
                return Ok(new ResultViewModel
                {
                    Message = "Configuração obtida com sucesso!",
                    Success = true,
                    Data = configuracao
                });
            });
        }

        // Salva/atualiza a preferência do usuário autenticado (upsert).
        [HttpPut("salvar")]
        [Authorize]
        public async Task<IActionResult> Salvar([FromBody] ConfiguracaoDashboardDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var salvo = await _service.Salvar(UsuarioAutenticado.Login, dto);
                return Ok(new ResultViewModel
                {
                    Message = "Painel salvo com sucesso!",
                    Success = true,
                    Data = salvo
                });
            });
        }
    }
}
