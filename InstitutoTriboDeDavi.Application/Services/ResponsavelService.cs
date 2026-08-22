using System;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class ResponsavelService : IResponsavelService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPresencaRepository _presencaRepository;
        private readonly IGraduacaoRepository _graduacaoRepository;
        private readonly IAvisoRepository _avisoRepository;
        private readonly IEventoCalendarioRepository _eventoRepository;
        private readonly IPoloRepository _poloRepository;

        // Só os avisos deste público-alvo aparecem para a família (0 = Todos).
        private const int PublicoTodos = 0;

        public ResponsavelService(
            IAlunoRepository alunoRepository,
            IPresencaRepository presencaRepository,
            IGraduacaoRepository graduacaoRepository,
            IAvisoRepository avisoRepository,
            IEventoCalendarioRepository eventoRepository,
            IPoloRepository poloRepository)
        {
            _alunoRepository = alunoRepository;
            _presencaRepository = presencaRepository;
            _graduacaoRepository = graduacaoRepository;
            _avisoRepository = avisoRepository;
            _eventoRepository = eventoRepository;
            _poloRepository = poloRepository;
        }

        public async Task<AcessoResponsavelDTO> AutenticarAsync(string codigo, DateTime dataNascimento)
        {
            var aluno = await _alunoRepository.ObterPorCodigoResponsavelAsync(codigo);

            // Código inexistente OU nascimento não confere → acesso negado. A
            // mesma resposta para os dois casos evita enumeração de códigos.
            if (aluno == null || aluno.DataNascimento.Date != dataNascimento.Date)
                return null;

            return new AcessoResponsavelDTO
            {
                AlunoId = aluno.Id,
                Nome = aluno.Nome,
                Faixa = (int)aluno.Faixa,
                Polo = await NomeDoPoloAsync(aluno.PoloId),
                Turma = aluno.Turma,
            };
        }

        public async Task<PainelResponsavelDTO> ObterPainelAsync(long alunoId)
        {
            var aluno = await _alunoRepository.GetByIdAsync(alunoId);
            if (aluno == null)
                return null;

            var presencas = await _presencaRepository.ObterPorAlunoAsync(alunoId);
            var total = presencas.Count;
            var presentes = presencas.Count(p => p.EstaPresente);
            var faltas = total - presentes;
            var percentual = total > 0 ? (int)Math.Round(100.0 * presentes / total) : 0;

            var graduacoes = await _graduacaoRepository.ListarPorAlunoAsync(alunoId);
            var avisos = (await _avisoRepository.ObterAtivosAsync())
                .Where(a => a.PublicoAlvo == PublicoTodos);

            var eventos = (await _eventoRepository.ObterPorAnoAsync(DateTime.Now.Year))
                .Where(e => e.PoloId == null || e.PoloId == aluno.PoloId);

            return new PainelResponsavelDTO
            {
                Aluno = new ResponsavelAlunoDTO
                {
                    Nome = aluno.Nome,
                    Faixa = (int)aluno.Faixa,
                    Polo = await NomeDoPoloAsync(aluno.PoloId),
                    Turma = aluno.Turma,
                    AutorizaImagem = aluno.AutorizaImagem,
                    AutorizaImagemEm = aluno.AutorizaImagemEm,
                },
                Frequencia = new FrequenciaResumoDTO
                {
                    TotalAulas = total,
                    Presencas = presentes,
                    Faltas = faltas,
                    Percentual = percentual,
                },
                // As presenças já vêm mais recentes primeiro; limita a lista.
                Presencas = presencas.Take(60).Select(p => new PresencaItemDTO
                {
                    Data = p.Data,
                    Presente = p.EstaPresente,
                }).ToList(),
                Graduacoes = graduacoes
                    .OrderByDescending(g => g.Data)
                    .Select(g => new GraduacaoItemDTO
                    {
                        Data = g.Data,
                        FaixaAnterior = g.FaixaAnterior,
                        FaixaNova = g.FaixaNova,
                    }).ToList(),
                Avisos = avisos
                    .OrderByDescending(a => a.DataCriacao)
                    .Select(a => new AvisoItemDTO
                    {
                        Titulo = a.Titulo,
                        Mensagem = a.Mensagem,
                        Data = a.DataCriacao,
                    }).ToList(),
                Eventos = eventos
                    .OrderBy(e => e.Data)
                    .Select(e => new EventoItemDTO
                    {
                        Data = e.Data,
                        DataFim = e.DataFim,
                        Titulo = e.Titulo,
                        Descricao = e.Descricao,
                        Tipo = e.Tipo,
                    }).ToList(),
            };
        }

        public async Task<bool> AtualizarAutorizacaoImagemAsync(long alunoId, bool autoriza)
        {
            var aluno = await _alunoRepository.GetByIdAsync(alunoId)
                ?? throw new DomainException("Aluno não encontrado.");

            aluno.AutorizaImagem = autoriza;
            aluno.AutorizaImagemEm = DateTime.Now;
            await _alunoRepository.UpdateAsync(aluno);

            return autoriza;
        }

        private async Task<string> NomeDoPoloAsync(long poloId)
        {
            if (poloId <= 0)
                return string.Empty;
            var polo = await _poloRepository.GetByIdAsync(poloId);
            return polo?.Nome ?? string.Empty;
        }
    }
}
