using AutoMapper;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]
    public class AniversarianteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAniversarianteService _aniversarianteService;

        public AniversarianteController(IMapper mapper, IAniversarianteService aniversarianteService)
        {
            _mapper = mapper;
            _aniversarianteService = aniversarianteService;
        }

        [HttpGet("aniversariantes/{mes}")]
        public async Task<IActionResult> GetAniversariantes(int mes)
        {
            var aniversariantes = await _aniversarianteService.GetAniversariantesAsync(mes);

            var aniversariantesComDescricao = aniversariantes.Select(static aluno => new
            {
                Nome = aluno.Nome,
                DataNascimento = aluno.DataNascimento,
                JaComemorado = aluno.JaComemorado,
            });

            if (aniversariantesComDescricao.Count() == 0)
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
                Message = "Faltas de alunos obtidas com sucesso!",
                Success = true,
                Data = aniversariantesComDescricao
            });
        }
    }
}
