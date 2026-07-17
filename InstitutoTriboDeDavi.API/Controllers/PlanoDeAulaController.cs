using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanoDeAulaController : BaseController
    {
        private readonly IPlanoDeAulaService _planoService;
        private readonly ILogger<PlanoDeAulaController> _logger;

        public PlanoDeAulaController(IPlanoDeAulaService planoService, ILogger<PlanoDeAulaController> logger) : base(logger)
        {
            _planoService = planoService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = nameof(UserRole.Administrador))]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var allPlanos = await _planoService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Planos de Aula encontrados com sucesso!",
                    Success = true,
                    Data = allPlanos
                });
            });
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public async Task<IActionResult> Get(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var plano = await _planoService.Get(id);

                if (plano == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Plano de Aula foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Plano de Aula encontrado com sucesso!",
                    Success = true,
                    Data = plano
                });
            });
        }

        [HttpGet("get-por-polo")]
        [Authorize]
        public async Task<IActionResult> ObterPlanos([FromQuery] IEnumerable<int> turmas)
        {
            return await ExecuteAsync(async () =>
            {
                var allPlanos = await _planoService.ObterPlanosTurmaAsync(UsuarioAutenticado, turmas);

                if (!allPlanos.Any())
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Plano de Aula foi encontrado!",
                        Success = true,
                        Data = null
                    });

                return Ok(new ResultViewModel
                {
                    Message = "Planos de Aula obtidos com sucesso!",
                    Success = true,
                    Data = allPlanos
                });
            });
        }

        [HttpPost("create")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Create([FromBody] PlanoDeAulaDTO planoDTO)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(planoDTO.PoloId);

                var planoCreated = await _planoService.Create(planoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Plano de Aula criado com sucesso!",
                    Success = true,
                    Data = planoCreated
                });
            });
        }

        [HttpPost("criar-de-modelo/{modeloId}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> CriarDeModelo(long modeloId, [FromBody] PlanoDeAulaDTO dadosBase)
        {
            return await ExecuteAsync(async () =>
            {
                ValidatePoloUsuario(dadosBase.PoloId);

                var planoCreated = await _planoService.CriarDeModelo(modeloId, dadosBase);

                return Ok(new ResultViewModel
                {
                    Message = "Plano de Aula criado a partir do modelo com sucesso!",
                    Success = true,
                    Data = planoCreated
                });
            });
        }

        [HttpPost("clonar/{id}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Clonar(long id, [FromBody] ClonarPlanoViewModel viewModel)
        {
            return await ExecuteAsync(async () =>
            {
                var original = await _planoService.Get(id);

                if (original == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Plano de Aula foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                ValidatePoloUsuario(original.PoloId);

                var clone = await _planoService.Clonar(id, viewModel.NovaDataPrevista);

                return Ok(new ResultViewModel
                {
                    Message = "Plano de Aula clonado com sucesso!",
                    Success = true,
                    Data = clone
                });
            });
        }

        [HttpPut("update")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Update([FromBody] PlanoDeAulaDTO planoDTO)
        {
            return await ExecuteAsync(async () =>
            {
                var existente = await _planoService.Get(planoDTO.Id);

                if (existente == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Plano de Aula foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                // Autorização sobre o registro existente (evita editar plano de outro polo)
                // e sobre o polo de destino (evita mover o plano para outro polo)
                ValidatePoloUsuario(existente.PoloId);
                ValidatePoloUsuario(planoDTO.PoloId);

                var planoUpdated = await _planoService.Update(planoDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Plano de Aula atualizado com sucesso!",
                    Success = true,
                    Data = planoUpdated
                });
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = AuthPolicies.ProfessorOuSuperior)]
        public async Task<IActionResult> Delete(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var plano = await _planoService.Get(id);

                if (plano == null)
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Plano de Aula foi encontrado com o ID informado!",
                        Success = true,
                        Data = null
                    });

                ValidatePoloUsuario(plano.PoloId);

                await _planoService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Plano de Aula removido com sucesso!",
                    Success = true,
                    Data = null
                });
            });
        }
    }
}
