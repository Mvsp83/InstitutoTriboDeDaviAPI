using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Galeria de vídeos (YouTube) do site. Listagem pública; inserir/remover
    // apenas administrador (canal do instituto).
    [ApiController]
    [Route("api/[controller]")]
    public class VideosGaleriaController : BaseController
    {
        private readonly IVideoGaleriaService _service;
        private readonly ILogger<VideosGaleriaController> _logger;

        public VideosGaleriaController(IVideoGaleriaService service, ILogger<VideosGaleriaController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Listar()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Vídeos obtidos com sucesso!",
                Success = true,
                Data = await _service.Listar()
            }));
        }

        [HttpPost("salvar")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Salvar([FromBody] VideoGaleriaDTO dto)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Vídeo salvo com sucesso!",
                Success = true,
                Data = await _service.Salvar(dto)
            }));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Excluir(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Excluir(id);
                return Ok(new ResultViewModel
                {
                    Message = "Vídeo removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
