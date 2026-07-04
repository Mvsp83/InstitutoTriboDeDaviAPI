using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AniversarianteController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAniversarianteService _aniversarianteService;
        private readonly ILogger<AniversarianteController> _logger;

        public AniversarianteController(IMapper mapper, IAniversarianteService aniversarianteService, ILogger<AniversarianteController> logger) : base(logger)
        {
            _mapper = mapper;
            _aniversarianteService = aniversarianteService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        [Route("aniversariantes/{mes}")]
        public async Task<IActionResult> GetAniversariantes(int mes)
        {
            return await ExecuteAsync(async () =>
            {
                var aniversariantes = await _aniversarianteService.GetAniversariantesAsync(UsuarioAutenticado, mes);

                var aniversariantesComDescricao = aniversariantes.Select(aluno => new
                {
                    Nome = aluno.Nome,
                    DataNascimento = aluno.DataNascimento,
                    JaComemorado = aluno.JaComemorado,
                });

                if (!aniversariantesComDescricao.Any())
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Aluno foi encontrado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Datas de aniversário de alunos obtidas com sucesso!",
                    Success = true,
                    Data = aniversariantesComDescricao
                });
            });
        }
    }
}
