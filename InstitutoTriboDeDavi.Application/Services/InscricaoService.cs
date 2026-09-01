using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.Common;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class InscricaoService : IInscricaoService
    {
        // Trava simples do endpoint público: mesmo WhatsApp não pode enviar
        // muitas fichas seguidas (erro de duplo clique ou envio automatizado).
        private const int LimiteEnviosPorJanela = 3;
        private const int JanelaMinutos = 10;

        private readonly IMapper _mapper;
        private readonly IInscricaoRepository _repository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPoloRepository _poloRepository;

        public InscricaoService(
            IMapper mapper,
            IInscricaoRepository repository,
            IAlunoRepository alunoRepository,
            IPoloRepository poloRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _alunoRepository = alunoRepository;
            _poloRepository = poloRepository;
        }

        public async Task<EnvioInscricaoResultado> Enviar(InscricaoDTO dto)
        {
            var inscricao = _mapper.Map<Inscricao>(dto);

            // Campos de controle nunca vêm de fora: quem manda é o servidor.
            inscricao.Id = 0;
            inscricao.Ano = DateTime.Today.Year;
            inscricao.DataEnvio = DateTime.UtcNow;
            inscricao.Status = (int)StatusInscricao.Pendente;
            inscricao.AlunoId = null;
            inscricao.DataRevisao = null;
            inscricao.RevisadoPor = string.Empty;
            inscricao.ObservacaoRevisao = string.Empty;
            // Código de acesso ao portal, entregue à família no fim do formulário.
            inscricao.CodigoResponsavel = CodigoAcesso.Gerar();

            inscricao.Validate();

            var poloEscolhido = await _poloRepository.GetByIdAsync(inscricao.PoloId);
            if (poloEscolhido == null)
                throw new DomainException("O polo selecionado não existe.");

            // Bloqueio por lotação: sem vaga, não aceita nova inscrição no polo.
            if (poloEscolhido.LimiteAlunos > 0 &&
                await _repository.ContarMatriculasAtivasAsync(inscricao.Ano, inscricao.PoloId)
                    >= poloEscolhido.LimiteAlunos)
                throw new DomainException(
                    "As vagas deste polo estão esgotadas no momento. Escolha outro polo ou fale com a equipe.");

            var recentes = await _repository.ContarEnviosRecentesAsync(
                inscricao.WhatsApp, JanelaMinutos);
            if (recentes >= LimiteEnviosPorJanela)
                throw new DomainException(
                    "Recebemos várias inscrições deste contato agora há pouco. " +
                    "Aguarde alguns minutos antes de enviar outra.");

            var criada = await _repository.CriarAsync(inscricao);
            return new EnvioInscricaoResultado(criada.Id, criada.CodigoResponsavel);
        }

        public async Task<List<InscricaoDTO>> Listar(int? status, int? ano, long? poloId)
        {
            var inscricoes = await _repository.ListarAsync(status, ano, poloId);
            var dtos = _mapper.Map<List<InscricaoDTO>>(inscricoes);

            // Nome do polo para a fila não precisar de outra consulta na tela.
            var polos = await _poloRepository.GetAllAsync();
            var nomes = polos.ToDictionary(p => p.Id, p => p.Nome);
            foreach (var d in dtos)
                d.PoloNome = nomes.TryGetValue(d.PoloId, out var n) ? n : "-";

            return dtos;
        }

        // ── Retenção / LGPD ───────────────────────────────────────────────
        // Inscrições recusadas são guardadas apenas por um período; depois disso
        // seus dados pessoais podem ser apagados (não viraram aluno).
        private const int MesesRetencaoRecusadas = 12;
        private static DateTime LimiteExpurgo() =>
            DateTime.Now.AddMonths(-MesesRetencaoRecusadas);

        public async Task<List<ExpurgoInscricaoDTO>> ListarExpurgoLgpd()
        {
            var lista = await _repository.ListarRecusadasParaExpurgoAsync(LimiteExpurgo());
            return lista.Select(i => new ExpurgoInscricaoDTO
            {
                Id = i.Id,
                Nome = i.Nome,
                Ano = i.Ano,
                DataEnvio = i.DataEnvio,
            }).ToList();
        }

        public async Task AnonimizarLgpd(long id)
        {
            var insc = await _repository.ObterAsync(id)
                ?? throw new DomainException("Inscrição não encontrada.");

            // Só recusadas e fora do prazo de retenção; idempotente se já feita.
            if (insc.Status != (int)StatusInscricao.Recusada)
                throw new DomainException("Só inscrições recusadas podem ser anonimizadas.");
            if (insc.DataEnvio >= LimiteExpurgo())
                throw new DomainException("Esta inscrição ainda está dentro do prazo de retenção.");
            if (insc.Anonimizada)
                return;

            insc.AnonimizarDados();
            await _repository.AtualizarAsync(insc);
        }

        public async Task<InscricaoDTO> Obter(long id)
        {
            var inscricao = await _repository.ObterAsync(id);
            if (inscricao == null)
                throw new DomainException("Inscrição não encontrada.");

            return _mapper.Map<InscricaoDTO>(inscricao);
        }

        public Task<int> ContarPendentes(long? poloId) =>
            _repository.ContarPendentesAsync(poloId);

        public async Task<MatriculaDTO> Aprovar(long id, RevisaoInscricaoDTO revisao, string revisor)
        {
            var inscricao = await _repository.ObterAsync(id);
            if (inscricao == null)
                throw new DomainException("Inscrição não encontrada.");
            if (inscricao.Status != (int)StatusInscricao.Pendente)
                throw new DomainException("Esta inscrição já foi revisada.");

            // O revisor confirma (ou corrige) polo e turma antes de aprovar.
            var poloId = revisao.PoloId > 0 ? revisao.PoloId : inscricao.PoloId;
            var turma = revisao.Turma > 0 ? revisao.Turma : (inscricao.Turma ?? 1);

            var polo = await _poloRepository.GetByIdAsync(poloId);
            if (polo == null)
                throw new DomainException("O polo informado não existe.");

            // Rematrícula: se o aluno já existe, atualizamos o cadastro dele em
            // vez de criar outro. Procura por CPF e, sem CPF, por nome +
            // data de nascimento.
            var aluno = await LocalizarAlunoExistente(inscricao);
            aluno ??= new Aluno();

            // Bloqueio por lotação ao aprovar. Não bloqueia rematrícula de quem
            // já ocupa vaga ativa neste polo/ano (não soma vaga nova).
            if (polo.LimiteAlunos > 0)
            {
                var matriculaAtual = aluno.Id > 0
                    ? await _repository.ObterMatriculaAsync(aluno.Id, inscricao.Ano)
                    : null;
                var jaOcupaVaga = matriculaAtual is { Ativa: true } && matriculaAtual.PoloId == poloId;
                if (!jaOcupaVaga &&
                    await _repository.ContarMatriculasAtivasAsync(inscricao.Ano, poloId) >= polo.LimiteAlunos)
                    throw new DomainException(
                        "Polo lotado: libere uma vaga (inative uma matrícula ou aumente o limite) antes de aprovar.");
            }

            PreencherAluno(aluno, inscricao, poloId, turma);
            // Transfere a foto da ficha (se houver) sem apagar a atual numa
            // rematrícula quando a inscrição veio sem foto. O revisor pode
            // descartar uma foto fora das diretrizes (não vai para o aluno).
            if (!revisao.DescartarFoto && !string.IsNullOrEmpty(inscricao.FotoArquivoId))
                aluno.FotoArquivoId = inscricao.FotoArquivoId;
            aluno.Validate();

            // A matrícula é validada no repositório, depois que o aluno é
            // gravado: só ali existe o AlunoId de um cadastro novo.
            var matricula = new Matricula
            {
                Ano = inscricao.Ano,
                PoloId = poloId,
                Turma = turma,
                InscricaoId = inscricao.Id,
                DataMatricula = DateTime.UtcNow,
                Ativa = true,
            };

            inscricao.Status = (int)StatusInscricao.Aprovada;
            inscricao.DataRevisao = DateTime.UtcNow;
            inscricao.RevisadoPor = revisor ?? string.Empty;
            inscricao.ObservacaoRevisao = revisao.Observacao ?? string.Empty;
            // Guarda o destino final, que pode ter sido corrigido na revisão.
            inscricao.PoloId = poloId;
            inscricao.Turma = turma;

            var (_, salva) = await _repository.AprovarAsync(inscricao, aluno, matricula);
            return _mapper.Map<MatriculaDTO>(salva);
        }

        public async Task Recusar(long id, string motivo, string revisor)
        {
            var inscricao = await _repository.ObterAsync(id);
            if (inscricao == null)
                throw new DomainException("Inscrição não encontrada.");
            if (inscricao.Status != (int)StatusInscricao.Pendente)
                throw new DomainException("Esta inscrição já foi revisada.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new DomainException("Informe o motivo da recusa.");

            inscricao.Status = (int)StatusInscricao.Recusada;
            inscricao.DataRevisao = DateTime.UtcNow;
            inscricao.RevisadoPor = revisor ?? string.Empty;
            inscricao.ObservacaoRevisao = motivo;

            await _repository.AtualizarAsync(inscricao);
        }

        public async Task<List<MatriculaDTO>> ListarMatriculas(int ano, long? poloId)
        {
            var matriculas = await _repository.ListarMatriculasAsync(ano, poloId);
            var dtos = _mapper.Map<List<MatriculaDTO>>(matriculas);

            // Nomes para exibição (batch: uma consulta de alunos e uma de polos).
            var alunos = (await _alunoRepository.GetAllAsync())
                .ToDictionary(a => a.Id, a => a.Nome);
            var polos = (await _poloRepository.GetAllAsync())
                .ToDictionary(p => p.Id, p => p.Nome);
            foreach (var dto in dtos)
            {
                dto.AlunoNome = alunos.TryGetValue(dto.AlunoId, out var an) ? an : "—";
                dto.PoloNome = polos.TryGetValue(dto.PoloId, out var pn) ? pn : "—";
            }
            return dtos.OrderBy(d => d.AlunoNome).ToList();
        }

        public async Task<MatriculaDTO> AlterarAtivaMatricula(long matriculaId, bool ativa)
        {
            var matricula = await _repository.ObterMatriculaPorIdAsync(matriculaId);
            if (matricula == null)
                throw new DomainException("Matrícula não encontrada.");

            if (ativa && !matricula.Ativa)
            {
                // Reativar reocupa vaga: respeita o limite do polo.
                var polo = await _poloRepository.GetByIdAsync(matricula.PoloId);
                if (polo != null && polo.LimiteAlunos > 0 &&
                    await _repository.ContarMatriculasAtivasAsync(matricula.Ano, matricula.PoloId)
                        >= polo.LimiteAlunos)
                    throw new DomainException(
                        "Polo lotado: não há vaga para reativar esta matrícula. Aumente o limite ou inative outra.");

                matricula.Ativa = true;
                matricula.DataEncerramento = null;
                matricula.MotivoEncerramento = string.Empty;
            }
            else if (!ativa && matricula.Ativa)
            {
                matricula.Ativa = false;
                matricula.DataEncerramento = DateTime.Now;
                matricula.MotivoEncerramento = "Inativada pela gestão (liberação de vaga).";
            }

            await _repository.AtualizarMatriculaAsync(matricula);
            return _mapper.Map<MatriculaDTO>(matricula);
        }

        public async Task<MatriculaLoteResultado> MatricularAno(int ano, long? poloId)
        {
            if (ano < 2000 || ano > 2100)
                throw new DomainException("Ano inválido.");

            // Base: alunos ativos que o usuário pode matricular (todos p/ admin,
            // ou só o polo do professor/supervisor).
            var alunos = poloId.HasValue
                ? await _alunoRepository.ObterPorPoloAsync(poloId.Value)
                : (await _alunoRepository.ObterTodosAsync())
                    .Where(a => a.AnonimizadoEm == null)
                    .ToList();

            var jaMatriculados = (await _repository.ObterAlunosMatriculadosAsync(ano, poloId))
                .ToHashSet();

            var novas = alunos
                .Where(a => !jaMatriculados.Contains(a.Id))
                .Select(a => new Matricula
                {
                    AlunoId = a.Id,
                    Ano = ano,
                    PoloId = a.PoloId,
                    Turma = a.Turma,
                    DataMatricula = DateTime.Now,
                    Ativa = true,
                })
                .ToList();

            var criadas = await _repository.CriarMatriculasAsync(novas);

            return new MatriculaLoteResultado(
                Criadas: criadas,
                JaMatriculados: alunos.Count - novas.Count,
                TotalAlunos: alunos.Count);
        }

        public async Task<DadosPreMatriculaDTO> BuscarParaRematricula(string cpfResponsavel, DateTime dataNascimento)
        {
            var cpf = SomenteDigitos(cpfResponsavel);
            if (cpf.Length == 0)
                return null;

            var alunos = await _alunoRepository.GetAllAsync();
            var aluno = alunos.FirstOrDefault(a =>
                a.AnonimizadoEm == null &&
                SomenteDigitos(a.CPFResponsavel) == cpf &&
                a.DataNascimento.Date == dataNascimento.Date);

            if (aluno == null)
                return null;

            return new DadosPreMatriculaDTO
            {
                AlunoId = aluno.Id,
                Nome = aluno.Nome ?? string.Empty,
                DataNascimento = aluno.DataNascimento.ToString("yyyy-MM-dd"),
                Rg = aluno.RG ?? string.Empty,
                Cpf = aluno.CPF ?? string.Empty,
                Peso = aluno.Peso,
                Altura = aluno.Altura,
                Faixa = (int)aluno.Faixa,
                Escola = aluno.Escola ?? string.Empty,
                Serie = aluno.Serie ?? string.Empty,
                Periodo = aluno.Periodo ?? string.Empty,
                Parentesco = (int)(aluno.Parentesco ?? 0),
                NomeResponsavel = aluno.Responsavel ?? string.Empty,
                RgResponsavel = aluno.RGResponsavel ?? string.Empty,
                CpfResponsavel = aluno.CPFResponsavel ?? string.Empty,
                Rua = aluno.Endereco ?? string.Empty,
                Numero = aluno.Numero ?? string.Empty,
                Complemento = aluno.Complemento ?? string.Empty,
                Bairro = aluno.Bairro ?? string.Empty,
                Cidade = aluno.Cidade ?? string.Empty,
                WhatsApp = aluno.Celular ?? string.Empty,
                Telefone2 = aluno.Telefone2 ?? string.Empty,
                PoloId = aluno.PoloId,
                TurmaAnterior = aluno.Turma,
            };
        }

        // ── Apoio ─────────────────────────────────────────────────────────
        private async Task<Aluno> LocalizarAlunoExistente(Inscricao inscricao)
        {
            var alunos = await _alunoRepository.GetAllAsync();

            var cpf = SomenteDigitos(inscricao.Cpf);
            if (!string.IsNullOrEmpty(cpf))
            {
                var porCpf = alunos.FirstOrDefault(a => SomenteDigitos(a.CPF) == cpf);
                if (porCpf != null) return porCpf;
            }

            return alunos.FirstOrDefault(a =>
                string.Equals(a.Nome?.Trim(), inscricao.Nome?.Trim(), StringComparison.OrdinalIgnoreCase)
                && a.DataNascimento.Date == inscricao.DataNascimento.Date);
        }

        private static string SomenteDigitos(string valor) =>
            new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());

        private static void PreencherAluno(Aluno aluno, Inscricao i, long poloId, int turma)
        {
            aluno.Nome = i.Nome;
            aluno.DataNascimento = i.DataNascimento;
            aluno.RG = i.Rg;
            aluno.CPF = i.Cpf;
            aluno.Peso = i.Peso.HasValue ? (double?)i.Peso.Value : null;
            aluno.Altura = i.Altura.HasValue ? (double?)i.Altura.Value : null;
            aluno.Faixa = (Faixa)i.Faixa;
            aluno.Escola = i.Escola;
            aluno.Serie = i.Serie;
            aluno.Periodo = i.Periodo;
            aluno.Responsavel = i.NomeResponsavel;
            aluno.RGResponsavel = i.RgResponsavel;
            aluno.CPFResponsavel = i.CpfResponsavel;
            aluno.Celular = i.WhatsApp;
            aluno.Telefone2 = i.Telefone2;
            aluno.Endereco = i.Rua;
            aluno.Numero = i.Numero;
            aluno.Complemento = i.Complemento;
            aluno.Bairro = i.Bairro;
            aluno.Cidade = i.Cidade;
            aluno.EhAdulto = i.Publico == 1;
            aluno.PoloId = poloId;
            aluno.Turma = turma;
            // Autorização de imagem informada pela família na ficha.
            aluno.AutorizaImagem = i.AceitouImagem;

            // Herda o código gerado na inscrição, para a família seguir usando o
            // mesmo. Se faltar (aluno antigo/edição manual), gera um agora.
            if (string.IsNullOrEmpty(aluno.CodigoResponsavel))
                aluno.CodigoResponsavel = string.IsNullOrEmpty(i.CodigoResponsavel)
                    ? CodigoAcesso.Gerar()
                    : i.CodigoResponsavel;

            if (Enum.IsDefined(typeof(Parentesco), i.Parentesco))
                aluno.Parentesco = (Parentesco)i.Parentesco;
        }
    }
}
