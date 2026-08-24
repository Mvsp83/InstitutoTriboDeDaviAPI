using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Apoio ao planejamento de aula: traz a transcrição (legenda) de um vídeo do
    // YouTube já traduzida para PT, para o professor colar na descrição.
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : BaseController
    {
        private readonly IVideoTranscricaoService _service;

        public VideoController(IVideoTranscricaoService service, ILogger<VideoController> logger) : base(logger)
        {
            _service = service;
        }

        [HttpGet("transcricao/{videoId}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Transcricao(string videoId)
        {
            return await ExecuteAsync(async () =>
            {
                var r = await _service.ObterTraduzidaAsync(videoId);
                return Ok(new ResultViewModel
                {
                    Message = string.IsNullOrEmpty(r.Aviso) ? "Transcrição obtida." : r.Aviso,
                    Success = string.IsNullOrEmpty(r.Aviso),
                    Data = new { texto = r.Texto, aviso = r.Aviso }
                });
            });
        }
    }
}
