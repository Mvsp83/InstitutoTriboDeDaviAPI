using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Acesso público aos balanços cadastrados, para a página de Transparência.
    // Só a categoria Balanço é exposta; DRE e Relatório de Atividades seguem
    // restritos no DocumentoContabilController (admin).
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class DocumentoPublicoController : BaseController
    {
        private readonly IDocumentoDriveService _documentoService;
        private readonly ILogger<DocumentoPublicoController> _logger;

        public DocumentoPublicoController(
            IDocumentoDriveService documentoService,
            ILogger<DocumentoPublicoController> logger) : base(logger)
        {
            _documentoService = documentoService;
            _logger = logger;
        }

        [HttpGet("balancos")]
        public async Task<IActionResult> Balancos()
        {
            return await ExecuteAsync(async () =>
            {
                var documentos = await _documentoService.ListarAsync(CategoriaDocumento.Balanco);
                var dados = documentos
                    .Select(d => new { d.Id, d.Nome, d.DataCriacao })
                    .ToList();

                return new OkObjectResult(new ResultViewModel
                {
                    Message = "Balanços.",
                    Success = true,
                    Data = dados
                });
            });
        }

        // Download binário (não envelopado). Só entrega se o arquivo pertencer à
        // categoria Balanço — impede acesso a DRE/Relatório pelo id.
        [HttpGet("balanco/{fileId}")]
        public async Task<IActionResult> BaixarBalanco(string fileId)
        {
            var balancos = await _documentoService.ListarAsync(CategoriaDocumento.Balanco);
            if (!balancos.Any(d => d.Id == fileId))
                return NotFound();

            var documento = await _documentoService.BaixarAsync(fileId);
            if (documento == null)
                return NotFound();

            return File(documento.Conteudo, documento.ContentType, documento.Nome);
        }
    }
}
