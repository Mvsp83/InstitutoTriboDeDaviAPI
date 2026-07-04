using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SincronizacaoController : BaseController
    {
        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly ISincronizacaoHistoricoRepository _historicoRepository;
        private readonly ILogger<SincronizacaoController> _logger;

        public SincronizacaoController(
            IGoogleSheetsService googleSheetsService,
            ISincronizacaoHistoricoRepository historicoRepository,
            ILogger<SincronizacaoController> logger) : base(logger)
        {
            _googleSheetsService = googleSheetsService;
            _historicoRepository = historicoRepository;
            _logger = logger;
        }

        [HttpPost("sincronizar-tudo")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> SincronizarTudo()
        {
            return await ExecuteAsync(async () =>
            {
                var resultados = await _googleSheetsService.SincronizarTodasAsPlanilhasAsync("Manual");

                return Ok(new ResultViewModel
                {
                    Message = $"Sincronização concluída: " +
                              $"{resultados.Sum(r => r.Inseridos)} inseridos, " +
                              $"{resultados.Sum(r => r.Atualizados)} atualizados.",
                    Success = true,
                    Data = resultados
                });
            });
        }

        [HttpPost("sincronizar-polo/{poloId}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> SincronizarPolo(long poloId)
        {
            return await ExecuteAsync(async () =>
            {
                var resultado = await _googleSheetsService.SincronizarPlanilhaAsync(poloId, "Manual");

                return Ok(new ResultViewModel
                {
                    Message = $"Polo {resultado.PoloNome}: " +
                              $"{resultado.Inseridos} inseridos, " +
                              $"{resultado.Atualizados} atualizados.",
                    Success = true,
                    Data = resultado
                });
            });
        }

        [HttpGet("historico")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ObterHistorico([FromQuery] int quantidade = 50)
        {
            return await ExecuteAsync(async () =>
            {
                var historico = await _historicoRepository.ObterUltimasAsync(quantidade);

                return Ok(new ResultViewModel
                {
                    Message = $"{historico.Count} registros encontrados.",
                    Success = true,
                    Data = historico
                });
            });
        }

        [HttpGet("historico/polo/{poloId}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ObterHistoricoPorPolo(long poloId, [FromQuery] int quantidade = 20)
        {
            return await ExecuteAsync(async () =>
            {
                var historico = await _historicoRepository.ObterPorPoloAsync(poloId, quantidade);

                return Ok(new ResultViewModel
                {
                    Message = $"{historico.Count} registros encontrados.",
                    Success = true,
                    Data = historico
                });
            });
        }

        [HttpGet("historico/ultima-execucao")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ObterUltimaExecucao()
        {
            return await ExecuteAsync(async () =>
            {
                var ultima = await _historicoRepository.ObterUltimaExecucaoAsync();

                if (ultima == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhuma sincronização encontrada.",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = $"Última execução: {ultima.DataExecucao:dd/MM/yyyy HH:mm} — " +
                              $"Polo: {ultima.PoloNome} — " +
                              $"Origem: {ultima.Origem}",
                    Success = true,
                    Data = ultima
                });
            });
        }
    }
}
