using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Administrador))]
    public class DocumentoOficialController : BaseController
    {
        private readonly IDocumentoOficialService _service;
        private readonly ILogger<DocumentoOficialController> _logger;

        public DocumentoOficialController(
            IDocumentoOficialService service,
            ILogger<DocumentoOficialController> logger) : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("ano/{ano}")]
        public async Task<IActionResult> ObterPorAno(int ano)
        {
            return await ExecuteAsync(async () =>
            {
                var docs = await _service.ObterPorAno(ano);
                return Ok(new ResultViewModel
                {
                    Message = "Documentos obtidos com sucesso!",
                    Success = true,
                    Data = docs
                });
            });
        }

        [HttpGet("anos")]
        public async Task<IActionResult> ObterAnos()
        {
            return await ExecuteAsync(async () =>
            {
                var anos = await _service.ObterAnos();
                return Ok(new ResultViewModel
                {
                    Message = "Anos obtidos com sucesso!",
                    Success = true,
                    Data = anos
                });
            });
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var doc = await _service.Get(id);
                return Ok(new ResultViewModel
                {
                    Message = doc == null ? "Não encontrado." : "Encontrado!",
                    Success = true,
                    Data = doc
                });
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] DocumentoOficialDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var criado = await _service.Create(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Rascunho criado com sucesso!",
                    Success = true,
                    Data = criado
                });
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] DocumentoOficialDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var atualizado = await _service.Update(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Rascunho atualizado com sucesso!",
                    Success = true,
                    Data = atualizado
                });
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Delete(id);
                return Ok(new ResultViewModel
                {
                    Message = "Documento removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // Aprovação: atribui o número oficial e trava o documento. Irreversível.
        [HttpPost("aprovar/{id}")]
        public async Task<IActionResult> Aprovar(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var aprovado = await _service.Aprovar(id);
                return Ok(new ResultViewModel
                {
                    Message = $"Documento aprovado com o número {aprovado.NumeroFormatado}.",
                    Success = true,
                    Data = aprovado
                });
            });
        }
    }
}
