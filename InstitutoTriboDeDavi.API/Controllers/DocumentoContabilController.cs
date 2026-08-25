using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using InstitutoTriboDeDavi.API.Utilities;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Armazenamento de documentos contábeis (DRE, Balanço, Relatório de
    // Atividades) no Google Drive. Exclusivo do Administrador.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Administrador))]
    [RequireModulo("financeiro")]
    public class DocumentoContabilController : BaseController
    {
        private readonly IDocumentoDriveService _documentoService;
        private readonly ILogger<DocumentoContabilController> _logger;

        public DocumentoContabilController(
            IDocumentoDriveService documentoService,
            ILogger<DocumentoContabilController> logger) : base(logger)
        {
            _documentoService = documentoService;
            _logger = logger;
        }

        [HttpGet("listar/{categoria}")]
        public async Task<IActionResult> Listar(string categoria)
        {
            return await ExecuteAsync(async () =>
            {
                var cat = ParseCategoria(categoria);
                var documentos = await _documentoService.ListarAsync(cat);

                return Ok(new ResultViewModel
                {
                    Message = $"{documentos.Count} documento(s) encontrado(s).",
                    Success = true,
                    Data = documentos
                });
            });
        }

        // 25 MB de folga: acima do limite de 20 MiB (20.971.520 bytes) validado
        // no front, para o arquivo no limite não ser rejeitado com 413 aqui.
        [HttpPost("upload/{categoria}")]
        [RequestSizeLimit(25_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 25_000_000)]
        public async Task<IActionResult> Upload(string categoria, IFormFile arquivo)
        {
            return await ExecuteAsync(async () =>
            {
                var cat = ParseCategoria(categoria);

                if (arquivo == null || arquivo.Length == 0)
                    throw new DomainException("Nenhum arquivo enviado.");

                await using var stream = arquivo.OpenReadStream();
                var criado = await _documentoService.UploadAsync(
                    cat, arquivo.FileName, arquivo.ContentType, stream);

                return Ok(new ResultViewModel
                {
                    Message = "Documento enviado com sucesso!",
                    Success = true,
                    Data = criado
                });
            });
        }

        // Retorno binário (não envelopado): o front baixa como blob.
        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> Download(string fileId)
        {
            var documento = await _documentoService.BaixarAsync(fileId);
            if (documento == null)
                return NotFound(new ResultViewModel
                {
                    Message = "Arquivo não encontrado.",
                    Success = false,
                    Data = null
                });

            return File(documento.Conteudo, documento.ContentType, documento.Nome);
        }

        [HttpDelete("excluir/{fileId}")]
        public async Task<IActionResult> Excluir(string fileId)
        {
            return await ExecuteAsync(async () =>
            {
                var removido = await _documentoService.ExcluirAsync(fileId);

                return Ok(new ResultViewModel
                {
                    Message = removido ? "Documento excluído." : "Documento não encontrado.",
                    Success = removido,
                    Data = null
                });
            });
        }

        private static CategoriaDocumento ParseCategoria(string categoria)
        {
            if (!Enum.TryParse<CategoriaDocumento>(categoria, ignoreCase: true, out var cat))
                throw new DomainException($"Categoria de documento inválida: {categoria}.");
            return cat;
        }
    }
}
