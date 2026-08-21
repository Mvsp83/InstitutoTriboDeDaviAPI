using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Financeiro do instituto: contas, lançamentos e transferências.
    // Restrito a Administrador — envolve prestação de contas da ONG.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Administrador))]
    public class FinanceiroController : BaseController
    {
        private readonly IFinanceiroService _service;
        private readonly ILogger<FinanceiroController> _logger;

        public FinanceiroController(IFinanceiroService service, ILogger<FinanceiroController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // ── Contas ────────────────────────────────────────────────────────
        [HttpGet("contas")]
        public async Task<IActionResult> ListarContas()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Contas obtidas com sucesso!",
                Success = true,
                Data = await _service.ListarContas()
            }));
        }

        [HttpPost("contas/salvar")]
        public async Task<IActionResult> SalvarConta([FromBody] ContaFinanceiraDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Conta salva com sucesso!",
                Success = true,
                Data = await _service.SalvarConta(dto)
            }));
        }

        [HttpDelete("contas/{id}")]
        public async Task<IActionResult> ExcluirConta(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ExcluirConta(id);
                return Ok(new ResultViewModel
                {
                    Message = "Conta removida com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // ── Movimentações ─────────────────────────────────────────────────
        [HttpGet("movimentacoes")]
        public async Task<IActionResult> ListarMovimentacoes()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Lançamentos obtidos com sucesso!",
                Success = true,
                Data = await _service.ListarMovimentacoes()
            }));
        }

        [HttpPost("movimentacoes/salvar")]
        public async Task<IActionResult> SalvarMovimentacao([FromBody] MovimentacaoFinanceiraDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Lançamento salvo com sucesso!",
                Success = true,
                Data = await _service.SalvarMovimentacao(dto)
            }));
        }

        [HttpDelete("movimentacoes/{id}")]
        public async Task<IActionResult> ExcluirMovimentacao(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ExcluirMovimentacao(id);
                return Ok(new ResultViewModel
                {
                    Message = "Lançamento removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpPut("movimentacoes/{id}/conciliacao")]
        public async Task<IActionResult> DefinirConciliacao(long id, [FromQuery] bool conciliado)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.DefinirConciliacao(id, conciliado);
                return Ok(new ResultViewModel
                {
                    Message = "Conciliação atualizada!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpPost("transferencias")]
        public async Task<IActionResult> RegistrarTransferencia([FromBody] TransferenciaDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.RegistrarTransferencia(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Transferência registrada com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // Migração única dos dados que estavam no navegador (localStorage).
        [HttpPost("importar")]
        public async Task<IActionResult> Importar([FromBody] ImportacaoFinanceiraDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var resultado = await _service.Importar(dto);
                return Ok(new ResultViewModel
                {
                    Message = resultado.Mensagem,
                    Success = true,
                    Data = resultado
                });
            });
        }
    }
}
