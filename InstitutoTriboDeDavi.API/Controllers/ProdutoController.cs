using System;
using System.IO;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Loja virtual: cadastro de produtos pelo admin e vitrine pública. A vitrine
    // e a foto são anônimas (qualquer visitante vê); o resto é só Administrador.
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : BaseController
    {
        private readonly IProdutoService _service;
        private readonly IFotoStorage _fotoStorage;
        private readonly ILogger<ProdutoController> _logger;

        public ProdutoController(
            IProdutoService service,
            IFotoStorage fotoStorage,
            ILogger<ProdutoController> logger) : base(logger)
        {
            _service = service;
            _fotoStorage = fotoStorage;
            _logger = logger;
        }

        // ── Público ─────────────────────────────────────────────────────────

        [HttpGet("vitrine")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "publico")]
        public async Task<IActionResult> Vitrine()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Vitrine.",
                Success = true,
                Data = await _service.Vitrine()
            }));
        }

        // Imagem do produto (bytes) — usada direto no <img> da vitrine pública.
        [HttpGet("{id}/foto")]
        [AllowAnonymous]
        public async Task<IActionResult> Foto(long id)
        {
            var produto = await _service.Obter(id);
            if (produto == null || string.IsNullOrEmpty(produto.FotoArquivoId))
                return NotFound();

            var download = await _fotoStorage.BaixarAsync(produto.FotoArquivoId);
            using var ms = new MemoryStream();
            await download.Conteudo.CopyToAsync(ms);
            return File(ms.ToArray(), download.ContentType);
        }

        // ── Admin ───────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Listar()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Produtos.",
                Success = true,
                Data = await _service.Listar()
            }));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Obter(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var produto = await _service.Obter(id);
                if (produto == null)
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Produto não encontrado.",
                        Success = false,
                        Data = null
                    });
                return Ok(new ResultViewModel { Message = "Produto.", Success = true, Data = produto });
            });
        }

        // Upload da foto do produto: guarda o binário e devolve o id do arquivo,
        // que o cadastro inclui no salvar.
        [HttpPost("foto")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        [RequestSizeLimit(15_000_000)]
        public async Task<IActionResult> UploadFoto(IFormFile arquivo)
        {
            return await ExecuteAsync(async () =>
            {
                if (arquivo == null || arquivo.Length == 0)
                    throw new DomainException("Nenhuma imagem enviada.");
                if (!(arquivo.ContentType ?? "").StartsWith("image/"))
                    throw new DomainException("O arquivo enviado não é uma imagem.");

                using var stream = arquivo.OpenReadStream();
                var fotoArquivoId = await _fotoStorage.UploadAsync(
                    arquivo.FileName, arquivo.ContentType, stream);

                return Ok(new ResultViewModel
                {
                    Message = "Foto recebida.",
                    Success = true,
                    Data = new { fotoArquivoId }
                });
            });
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Criar([FromBody] ProdutoDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Produto cadastrado.",
                Success = true,
                Data = await _service.Criar(dto)
            }));
        }

        [HttpPut]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Atualizar([FromBody] ProdutoDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Produto atualizado.",
                Success = true,
                Data = await _service.Atualizar(dto)
            }));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Excluir(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var produto = await _service.Obter(id);
                await _service.Excluir(id);
                // Remove a foto órfã do storage (best-effort).
                if (produto != null && !string.IsNullOrEmpty(produto.FotoArquivoId))
                {
                    try { await _fotoStorage.ExcluirAsync(produto.FotoArquivoId); }
                    catch (Exception e) { _logger.LogWarning(e, "Falha ao excluir a foto do produto {Id}.", id); }
                }
                return Ok(new ResultViewModel { Message = "Produto excluído.", Success = true, Data = null });
            });
        }
    }
}
