using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Consulta do log de auditoria. Só Administrador: é justamente quem precisa
    // fiscalizar quem alterou financeiro, doações e cadastros.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Administrador))]
    public class LogAuditoriaController : BaseController
    {
        private readonly ILogAuditoriaService _service;
        private readonly ILogger<LogAuditoriaController> _logger;

        public LogAuditoriaController(ILogAuditoriaService service, ILogger<LogAuditoriaController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] string entidade,
            [FromQuery] string usuario,
            [FromQuery] int limite = 100)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Log obtido com sucesso!",
                Success = true,
                Data = await _service.Listar(entidade, usuario, limite)
            }));
        }
    }
}
