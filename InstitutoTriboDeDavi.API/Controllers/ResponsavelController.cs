using InstitutoTriboDeDavi.API.Token.Interfaces;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Portal do responsável: superfície pública, só leitura, escopada a um
    // aluno pelo código de acesso + data de nascimento.
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsavelController : BaseController
    {
        private readonly IResponsavelService _responsavelService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<ResponsavelController> _logger;

        public ResponsavelController(IResponsavelService responsavelService, ITokenGenerator tokenGenerator, ILogger<ResponsavelController> logger) : base(logger)
        {
            _responsavelService = responsavelService;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        // Login do responsável: código + nascimento → token só-leitura de 4h.
        [HttpPost("acesso")]
        [AllowAnonymous]
        [EnableRateLimiting(AuthPolicies.LoginRateLimit)]
        public async Task<IActionResult> Acesso([FromBody] AcessoResponsavelViewModel model)
        {
            return await ExecuteAsync(async () =>
            {
                var acesso = await _responsavelService.AutenticarAsync(model.Codigo, model.DataNascimento);

                if (acesso == null)
                {
                    _logger.LogWarning("Acesso de responsável negado (código/nascimento inválidos).");
                    return StatusCode(401, Responses.UnauthorizedErrorMessage());
                }

                var token = _tokenGenerator.GenerateResponsavelToken(acesso.AlunoId, acesso.Nome);

                return Ok(new ResultViewModel
                {
                    Message = "Acesso liberado.",
                    Success = true,
                    Data = new
                    {
                        Token = token,
                        TokenExpires = DateTime.UtcNow.AddHours(4),
                        Aluno = new
                        {
                            acesso.Nome,
                            acesso.Faixa,
                            acesso.Polo,
                            acesso.Turma,
                        }
                    }
                });
            });
        }

        // Painel do aluno vinculado ao token. Só o papel "Responsavel" acessa.
        [HttpGet("painel")]
        [Authorize(Roles = "Responsavel")]
        public async Task<IActionResult> Painel()
        {
            return await ExecuteAsync(async () =>
            {
                var claim = User.FindFirst("AlunoId")?.Value;
                if (!long.TryParse(claim, out var alunoId))
                    return StatusCode(401, Responses.UnauthorizedErrorMessage());

                var painel = await _responsavelService.ObterPainelAsync(alunoId);
                if (painel == null)
                    return StatusCode(404, new ResultViewModel
                    {
                        Message = "Aluno não encontrado.",
                        Success = false,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Painel obtido com sucesso!",
                    Success = true,
                    Data = painel
                });
            });
        }
    }
}
