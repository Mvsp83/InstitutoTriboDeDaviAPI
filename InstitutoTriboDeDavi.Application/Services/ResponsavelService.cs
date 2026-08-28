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
        private readonly IOcorrenciaAlunoRepository _ocorrenciaRepository;
        private readonly IFotoStorage _fotoStorage;

        // Tipos de OcorrenciaAluno (espelham o service): 0 = advertência, 1 = recado.
        private const int TipoAdvertencia = 0;
        private const int TipoRecado = 1;

        // Só os avisos deste público-alvo aparecem para a família (0 = Todos).
        private const int PublicoTodos = 0;

        public ResponsavelService(
            IAlunoRepository alunoRepository,
            IPresencaRepository presencaRepository,
            IGraduacaoRepository graduacaoRepository,
            IAvisoRepository avisoRepository,
            IEventoCalendarioRepository eventoRepository,
            IPoloRepository poloRepository,
            IOcorrenciaAlunoRepository ocorrenciaRepository,
            IFotoStorage fotoStorage)
        {
            _alunoRepository = alunoRepository;
            _presencaRepository = presencaRepository;
            _graduacaoRepository = graduacaoRepository;
            _avisoRepository = avisoRepository;
            _eventoRepository = eventoRepository;
            _poloRepository = poloRepository;
            _ocorrenciaRepository = ocorrenciaRepository;
            _fotoStorage = fotoStorage;
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

            var ocorrencias = await _ocorrenciaRepository.ListarPorAlunoAsync(alunoId);

            var fotoDataUri = await ObterFotoDataUriAsync(aluno.FotoArquivoId);

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
                    FotoDataUri = fotoDataUri,
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
                    Id = p.Id,
                    Data = p.Data,
                    Presente = p.EstaPresente,
                    Justificativa = p.JustificativaResponsavel,
                    JustificadaEm = p.JustificadaEm,
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
                Advertencias = ocorrencias
                    .Where(o => o.Tipo == TipoAdvertencia)
                    .Select(o => new AdvertenciaItemDTO { Data = o.Data, Motivo = o.Texto })
                    .ToList(),
                Recados = ocorrencias
                    .Where(o => o.Tipo == TipoRecado)
                    .Select(o => new RecadoItemDTO { Data = o.Data, Status = o.Status, Texto = o.Texto })
                    .ToList(),
            };
        }

        // Tamanho máximo da justificativa — cabe um bilhete do responsável sem
        // virar campo de texto livre gigante.
        private const int JustificativaMaxLength = 500;

        public async Task<PresencaItemDTO> JustificarFaltaAsync(long alunoId, long presencaId, string justificativa)
        {
            var texto = (justificativa ?? string.Empty).Trim();
            if (texto.Length == 0)
                throw new DomainException("Escreva o motivo da falta.");
            if (texto.Length > JustificativaMaxLength)
                throw new DomainException($"A justificativa deve ter no máximo {JustificativaMaxLength} caracteres.");

            var presenca = await _presencaRepository.GetByIdAsync(presencaId);

            // Presença inexistente OU de outro aluno → mesma resposta, para não
            // vazar a existência de registros de terceiros pelo token do portal.
            if (presenca == null || presenca.AlunoId != alunoId)
                throw new DomainException("Falta não encontrada.");

            if (presenca.EstaPresente)
                throw new DomainException("Esse registro é uma presença, não uma falta.");

            presenca.JustificativaResponsavel = texto;
            presenca.JustificadaEm = DateTime.Now;
            await _presencaRepository.UpdateAsync(presenca);

            return new PresencaItemDTO
            {
                Id = presenca.Id,
                Data = presenca.Data,
                Presente = presenca.EstaPresente,
                Justificativa = presenca.JustificativaResponsavel,
                JustificadaEm = presenca.JustificadaEm,
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

        // Foto do aluno em base64 para o portal — só quando a config global
        // permite mostrá-la no responsável e o aluno tem foto.
        private async Task<string> ObterFotoDataUriAsync(string fotoArquivoId)
        {
            if (string.IsNullOrEmpty(fotoArquivoId))
                return null;

            var cfg = await _alunoRepository.ObterConfigFotoAsync();
            // Ausente = padrão (mostra).
            if (cfg != null && !cfg.MostrarNoResponsavel)
                return null;

            var download = await _fotoStorage.BaixarAsync(fotoArquivoId);
            if (download == null)
                return null;

            using var ms = new System.IO.MemoryStream();
            await download.Conteudo.CopyToAsync(ms);
            var base64 = System.Convert.ToBase64String(ms.ToArray());
            return $"data:{download.ContentType};base64,{base64}";
        }
    }
}
