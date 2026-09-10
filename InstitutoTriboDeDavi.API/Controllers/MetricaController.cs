using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricaController : BaseController
    {
        private readonly IMetricaService _metricaService;

        public MetricaController(IMetricaService metricaService, ILogger<MetricaController> logger) : base(logger)
        {
            _metricaService = metricaService;
        }

        // Beacon público: o site chama isto para contabilizar um acesso/evento.
        // Sempre responde 200 (nunca atrapalha a navegação do visitante).
        [HttpPost("evento")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.MetricaRateLimit)]
        public async Task<IActionResult> Registrar([FromBody] MetricaEventoDTO evento)
        {
            return await ExecuteAsync(async () =>
            {
                await _metricaService.RegistrarAsync(evento);
                return Ok(new ResultViewModel { Message = "ok", Success = true, Data = null });
            });
        }

        // Resumo agregado para a tela "Acessos ao site" (admin).
        [HttpGet("resumo")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Resumo([FromQuery] int dias = 30)
        {
            return await ExecuteAsync(async () =>
            {
                var resumo = await _metricaService.ObterResumoAsync(dias);
                return Ok(new ResultViewModel
                {
                    Message = "Resumo de acessos.",
                    Success = true,
                    Data = resumo
                });
            });
        }
    }
}
