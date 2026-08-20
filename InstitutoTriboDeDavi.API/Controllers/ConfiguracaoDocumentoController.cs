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
    public class ConfiguracaoDocumentoController : BaseController
    {
        private readonly IConfiguracaoDocumentoService _service;
        private readonly ILogger<ConfiguracaoDocumentoController> _logger;

        public ConfiguracaoDocumentoController(
            IConfiguracaoDocumentoService service,
            ILogger<ConfiguracaoDocumentoController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // Qualquer usuário autenticado lê o padrão (usado ao exportar documentos).
        [HttpGet("obter")]
        [Authorize]
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

        // Só Administrador altera o padrão dos documentos.
        [HttpPut("salvar")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Salvar([FromBody] ConfiguracaoDocumentoDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var salvo = await _service.Salvar(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Padrão de documentos salvo com sucesso!",
                    Success = true,
                    Data = salvo
                });
            });
        }
    }
}
