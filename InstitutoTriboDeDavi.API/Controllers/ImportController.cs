using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.Import;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : BaseController
    {
        private readonly IFactoryPlanilhaDB _factory;
        private readonly ILogger<ImportController> _logger;

        public ImportController(IFactoryPlanilhaDB factory, ILogger<ImportController> logger) : base(logger)
        {
            _factory = factory;
            _logger = logger;
        }

        [HttpPost("alunos")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ImportarAlunos(IFormFile arquivo, [FromQuery] long poloIdPadrao = 1)
        {
            return await ExecuteAsync(async () =>
            {
                if (arquivo == null || arquivo.Length == 0)
                    return BadRequest(new ResultViewModel
                    {
                        Message = "Nenhum arquivo enviado.",
                        Success = false,
                        Data = null
                    });

                using var stream = arquivo.OpenReadStream();
                var resultado = await _factory.ImportarAlunosAsync(stream, poloIdPadrao);

                return Ok(new ResultViewModel
                {
                    Message = $"Importação concluída: {resultado.Inseridos} inseridos, " +
                              $"{resultado.Atualizados} atualizados, {resultado.Ignorados} ignorados.",
                    Success = true,
                    Data = resultado
                });
            });
        }
    }
}
