using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace InstitutoTriboDeDavi.API.Controllers
{
    // Diretoria e Conselho Fiscal por ano de vigência. O admin cadastra; a
    // página pública de Transparência lê pelo endpoint público.
    [ApiController]
    [Route("api/[controller]")]
    public class GovernancaController : BaseController
    {
        private readonly IGovernancaService _service;

        public GovernancaController(IGovernancaService service, ILogger<GovernancaController> logger)
            : base(logger)
        {
            _service = service;
        }

        [HttpGet("anos")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Anos()
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Anos obtidos.",
                Success = true,
                Data = await _service.ListarAnos()
            }));
        }

        [HttpGet("{ano:int}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> PorAno(int ano)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Membros obtidos.",
                Success = true,
                Data = await _service.ListarPorAno(ano)
            }));
        }

        [HttpPut("{ano:int}")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> Salvar(int ano, [FromBody] List<MembroGovernancaDTO> membros)
        {
            return await ExecuteAsync(async () =>
            {
                await _service.Salvar(ano, membros);
                return Ok(new ResultViewModel
                {
                    Message = "Governança salva.",
                    Success = true,
                    Data = null
                });
            });
        }

        // Público: membros de um ano (ou o mais recente) + anos disponíveis.
        [HttpGet("publico")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "publico")]
        public async Task<IActionResult> Publico([FromQuery] int? ano)
        {
            return await ExecuteAsync(async () => Ok(new ResultViewModel
            {
                Message = "Governança.",
                Success = true,
                Data = await _service.ObterPublico(ano)
            }));
        }
    }
}
