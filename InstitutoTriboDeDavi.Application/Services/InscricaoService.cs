using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        public async Task<long> Enviar(InscricaoDTO dto)
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

            inscricao.Validate();

            if (await _poloRepository.GetByIdAsync(inscricao.PoloId) == null)
                throw new DomainException("O polo selecionado não existe.");

            var recentes = await _repository.ContarEnviosRecentesAsync(
                inscricao.WhatsApp, JanelaMinutos);
            if (recentes >= LimiteEnviosPorJanela)
                throw new DomainException(
                    "Recebemos várias inscrições deste contato agora há pouco. " +
                    "Aguarde alguns minutos antes de enviar outra.");

            var criada = await _repository.CriarAsync(inscricao);
            return criada.Id;
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

            if (await _poloRepository.GetByIdAsync(poloId) == null)
                throw new DomainException("O polo informado não existe.");

            // Rematrícula: se o aluno já existe, atualizamos o cadastro dele em
            // vez de criar outro. Procura por CPF e, sem CPF, por nome +
            // data de nascimento.
            var aluno = await LocalizarAlunoExistente(inscricao);
            aluno ??= new Aluno();

            PreencherAluno(aluno, inscricao, poloId, turma);
            aluno.Validate();

            var matricula = new Matricula
            {
                Ano = inscricao.Ano,
                PoloId = poloId,
                Turma = turma,
                InscricaoId = inscricao.Id,
                DataMatricula = DateTime.UtcNow,
                Ativa = true,
            };
            matricula.Validate();

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
            return _mapper.Map<List<MatriculaDTO>>(matriculas);
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
            aluno.Faixa = (Faixa)i.Faixa;
            aluno.Escola = i.Escola;
            aluno.Periodo = i.Periodo;
            aluno.Responsavel = i.NomeResponsavel;
            aluno.RGResponsavel = i.RgResponsavel;
            aluno.CPFResponsavel = i.CpfResponsavel;
            aluno.Celular = i.WhatsApp;
            aluno.Bairro = i.Bairro;
            aluno.Cidade = i.Cidade;
            aluno.PoloId = poloId;
            aluno.Turma = turma;

            if (Enum.IsDefined(typeof(Parentesco), i.Parentesco))
                aluno.Parentesco = (Parentesco)i.Parentesco;

            // O cadastro guarda o endereço numa linha só; a ficha vem separada.
            var partes = new[] { i.Rua, i.Numero, i.Complemento }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            aluno.Endereco = string.Join(", ", partes);
        }
    }
}
