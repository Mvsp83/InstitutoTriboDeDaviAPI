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
    // Graduações de faixa. Leitura para qualquer usuário autenticado; registrar
    // e excluir exigem Professor ou superior, restrito ao próprio polo.
    [ApiController]
    [Route("api/[controller]")]
    public class GraduacaoController : BaseController
    {
        private readonly IGraduacaoService _service;
        private readonly ILogger<GraduacaoController> _logger;

        public GraduacaoController(IGraduacaoService service, ILogger<GraduacaoController> logger)
            : base(logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Listar([FromQuery] int? ano)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Graduações obtidas com sucesso!",
                Success = true,
                Data = await _service.Listar(ano, PoloDoUsuario())
            }));
        }

        [HttpGet("aluno/{alunoId}")]
        [Authorize]
        public async Task<IActionResult> PorAluno(long alunoId)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Histórico obtido com sucesso!",
                Success = true,
                Data = await _service.ListarPorAluno(alunoId)
            }));
        }

        [HttpPost("registrar")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Registrar([FromBody] GraduacaoLoteDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                var resultado = await _service.Registrar(dto, UsuarioAutenticado.Login);

                return Ok(new ResultViewModel
                {
                    Message = resultado.Mensagem,
                    Success = true,
                    Data = resultado
                });
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Excluir(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var graduacao = await _service.Obter(id);
                ValidatePoloUsuario(graduacao.PoloId);

                await _service.Excluir(id);

                return Ok(new ResultViewModel
                {
                    Message = "Graduação removida e faixa anterior restaurada.",
                    Success = true,
                    Data = null
                });
            });
        }

        private long? PoloDoUsuario() =>
            UsuarioAutenticado.Role == UserRole.Administrador
                ? null
                : UsuarioAutenticado.PoloId;
    }
}
