using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using InstitutoTriboDeDavi.API.Utilities;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Doadores e doações. Restrito a Administrador: envolve dados pessoais de
    // apoiadores e a prestação de contas da ONG.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Administrador))]
    [RequireModulo("captacao")]
    public class DoacaoController : BaseController
    {
        private readonly IDoacaoService _service;
        private readonly ILogger<DoacaoController> _logger;

        public DoacaoController(IDoacaoService service, ILogger<DoacaoController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        // ── Doadores ──────────────────────────────────────────────────────
        [HttpGet("doadores")]
        public async Task<IActionResult> ListarDoadores()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Doadores obtidos com sucesso!",
                Success = true,
                Data = await _service.ListarDoadores()
            }));
        }

        [HttpPost("doadores/salvar")]
        public async Task<IActionResult> SalvarDoador([FromBody] DoadorDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Doador salvo com sucesso!",
                Success = true,
                Data = await _service.SalvarDoador(dto)
            }));
        }

        [HttpDelete("doadores/{id}")]
        public async Task<IActionResult> ExcluirDoador(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ExcluirDoador(id);
                return Ok(new ResultViewModel
                {
                    Message = "Doador removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // ── Doações ───────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ListarDoacoes([FromQuery] int? ano, [FromQuery] long? doadorId)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Doações obtidas com sucesso!",
                Success = true,
                Data = await _service.ListarDoacoes(ano, doadorId)
            }));
        }

        [HttpGet("resumo/{ano}")]
        public async Task<IActionResult> Resumo(int ano)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Resumo obtido com sucesso!",
                Success = true,
                Data = await _service.Resumo(ano)
            }));
        }

        [HttpPost("salvar")]
        public async Task<IActionResult> SalvarDoacao([FromBody] DoacaoDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Doação registrada com sucesso!",
                Success = true,
                Data = await _service.SalvarDoacao(dto, UsuarioAutenticado.Login)
            }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirDoacao(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.ExcluirDoacao(id);
                return Ok(new ResultViewModel
                {
                    Message = "Doação removida com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        [HttpPost("{id}/recibo")]
        public async Task<IActionResult> EmitirRecibo(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var doacao = await _service.EmitirRecibo(id);
                return Ok(new ResultViewModel
                {
                    Message = $"Recibo {doacao.ReciboNumero} emitido!",
                    Success = true,
                    Data = doacao
                });
            });
        }
    }
}
