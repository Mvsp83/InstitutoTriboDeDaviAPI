using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Mensalidades: planos, matrículas financeiras e cobranças. Restrito a
    // Administrador e ao módulo financeiro (mesma prestação de contas).
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Administrador))]
    [RequireModulo("financeiro")]
    public class MensalidadesController : BaseController
    {
        private readonly IMensalidadesService _service;
        private readonly ILogger<MensalidadesController> _logger;

        public MensalidadesController(IMensalidadesService service, ILogger<MensalidadesController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // ── Planos ────────────────────────────────────────────────────────
        [HttpGet("planos")]
        public async Task<IActionResult> ListarPlanos()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Planos obtidos com sucesso!",
                Success = true,
                Data = await _service.ListarPlanos()
            }));
        }

        [HttpPost("planos/salvar")]
        public async Task<IActionResult> SalvarPlano([FromBody] PlanoMensalidadeDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Plano salvo com sucesso!",
                Success = true,
                Data = await _service.SalvarPlano(dto)
            }));
        }

        [HttpDelete("planos/{id}")]
        public async Task<IActionResult> ExcluirPlano(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ExcluirPlano(id);
                return Ok(new ResultViewModel
                {
                    Message = "Plano removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // ── Matrículas ────────────────────────────────────────────────────
        [HttpGet("matriculas")]
        public async Task<IActionResult> ListarMatriculas()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Matrículas obtidas com sucesso!",
                Success = true,
                Data = await _service.ListarMatriculas()
            }));
        }

        [HttpPost("matriculas/salvar")]
        public async Task<IActionResult> SalvarMatricula([FromBody] MatriculaFinanceiraDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Matrícula salva com sucesso!",
                Success = true,
                Data = await _service.SalvarMatricula(dto)
            }));
        }

        [HttpDelete("matriculas/{id}")]
        public async Task<IActionResult> ExcluirMatricula(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ExcluirMatricula(id);
                return Ok(new ResultViewModel
                {
                    Message = "Matrícula removida com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // ── Cobranças ─────────────────────────────────────────────────────
        [HttpGet("cobrancas")]
        public async Task<IActionResult> ListarCobrancas([FromQuery] string competencia)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Cobranças obtidas com sucesso!",
                Success = true,
                Data = await _service.ListarCobrancas(competencia)
            }));
        }

        [HttpPost("cobrancas/gerar")]
        public async Task<IActionResult> GerarCobrancas([FromBody] GerarCobrancasDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var resultado = await _service.GerarCobrancas(dto.Competencia);
                return Ok(new ResultViewModel
                {
                    Message = resultado.Mensagem,
                    Success = true,
                    Data = resultado
                });
            });
        }

        [HttpPost("cobrancas/baixar")]
        public async Task<IActionResult> BaixarCobranca([FromBody] BaixaCobrancaDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Baixa registrada e lançada no livro-caixa!",
                Success = true,
                Data = await _service.Baixar(dto)
            }));
        }

        [HttpPost("cobrancas/salvar")]
        public async Task<IActionResult> SalvarCobranca([FromBody] CobrancaDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Cobrança salva com sucesso!",
                Success = true,
                Data = await _service.SalvarCobranca(dto)
            }));
        }

        [HttpDelete("cobrancas/{id}")]
        public async Task<IActionResult> ExcluirCobranca(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ExcluirCobranca(id);
                return Ok(new ResultViewModel
                {
                    Message = "Cobrança removida com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
