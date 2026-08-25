using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using InstitutoTriboDeDavi.API.Utilities;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequireModulo("relacionamento")]
    public class EventoCalendarioController : BaseController
    {
        // Escrita restrita a Administrador e Supervisor; leitura a qualquer
        // usuário autenticado.
        private const string GERENCIA =
            nameof(UserRole.Administrador) + "," + nameof(UserRole.Supervisor);

        private readonly IEventoCalendarioService _service;
        private readonly INotificacaoCalendarioService _notificacaoService;
        private readonly ILogger<EventoCalendarioController> _logger;

        public EventoCalendarioController(
            IEventoCalendarioService service,
            INotificacaoCalendarioService notificacaoService,
            ILogger<EventoCalendarioController> logger) : base(logger)
        {
            _service = service;
            _notificacaoService = notificacaoService;
            _logger = logger;
        }

        [HttpGet("ano/{ano}")]
        [Authorize]
        public async Task<IActionResult> ObterPorAno(int ano)
        {
            return await ExecuteAsync(async () =>
            {
                var eventos = await _service.ObterPorAno(ano);
                return Ok(new ResultViewModel
                {
                    Message = "Eventos obtidos com sucesso!",
                    Success = true,
                    Data = eventos
                });
            });
        }

        [HttpGet("anos")]
        [Authorize]
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

        [HttpPost("create")]
        [Authorize(Roles = GERENCIA)]
        public async Task<IActionResult> Create([FromBody] EventoCalendarioDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var criado = await _service.Create(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Evento criado com sucesso!",
                    Success = true,
                    Data = criado
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Roles = GERENCIA)]
        public async Task<IActionResult> Update([FromBody] EventoCalendarioDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var atualizado = await _service.Update(dto);
                return Ok(new ResultViewModel
                {
                    Message = "Evento atualizado com sucesso!",
                    Success = true,
                    Data = atualizado
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = GERENCIA)]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Delete(id);
                return Ok(new ResultViewModel
                {
                    Message = "Evento removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }

        // Dispara AGORA o processamento dos avisos por email (todos os eventos
        // com notificação pendente, ignorando a janela de data). Só Administrador.
        // Devolve o resumo com os erros por evento (ex.: SMTP não configurado).
        [HttpPost("processar-avisos")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> ProcessarAvisos()
        {
            return await ExecuteAsync(async () =>
            {
                var resultado = await _notificacaoService.ProcessarAsync(forcarEnvio: true);

                var mensagem = resultado.Erros.Count > 0
                    ? $"{resultado.Enviados} enviado(s), {resultado.Erros.Count} com erro."
                    : $"{resultado.Enviados} aviso(s) enviado(s).";

                // Sempre Success = true: o processamento rodou. Os erros por
                // evento vão no Data para a tela exibir o motivo (ex.: SMTP).
                return Ok(new ResultViewModel
                {
                    Message = mensagem,
                    Success = true,
                    Data = resultado
                });
            });
        }

        [HttpPost("copiar/{anoOrigem}/{anoDestino}")]
        [Authorize(Roles = GERENCIA)]
        public async Task<IActionResult> CopiarAno(int anoOrigem, int anoDestino)
        {
            return await ExecuteAsync(async () =>
            {
                var total = await _service.CopiarAno(anoOrigem, anoDestino);
                return Ok(new ResultViewModel
                {
                    Message = $"{total} evento(s) copiado(s) para {anoDestino}.",
                    Success = true,
                    Data = total
                });
            });
        }
    }
}
