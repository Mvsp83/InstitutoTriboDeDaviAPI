using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracaoLojaController : BaseController
    {
        private readonly IConfiguracaoLojaService _service;
        private readonly ILogger<ConfiguracaoLojaController> _logger;

        public ConfiguracaoLojaController(
            IConfiguracaoLojaService service,
            ILogger<ConfiguracaoLojaController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // Público: a loja do site lê o número/flag para montar o botão de compra.
        [HttpGet("obter")]
        [AllowAnonymous]
        public async Task<IActionResult> Obter()
        {
            return await ExecuteAsync(async () =>
            {
                var configuracao = await _service.Obter();
                return Ok(new ResultViewModel
                {
                    Message = "Configuração obtida com sucesso!",
                    Success = true,
                    Data = configuracao
                });
            });
        }

        // Só o admin altera o número e liga/desliga a compra por WhatsApp.
        [HttpPut("salvar")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Salvar([FromBody] ConfiguracaoLojaDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var salvo = await _service.Salvar(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Configuração da loja salva com sucesso!",
                    Success = true,
                    Data = salvo
                });
            });
        }
    }
}
