using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AtletaService : IAtletaService
    {
        private readonly IMapper _mapper;
        private readonly IAtletaRepository _repository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPoloRepository _poloRepository;

        public AtletaService(
            IMapper mapper,
            IAtletaRepository repository,
            IAlunoRepository alunoRepository,
            IPoloRepository poloRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _alunoRepository = alunoRepository;
            _poloRepository = poloRepository;
        }

        public async Task<List<AtletaDTO>> Listar()
        {
            var atletas = await _repository.ListarAsync();
            var dtos = _mapper.Map<List<AtletaDTO>>(atletas);

            var alunos = (await _alunoRepository.GetAllAsync()).ToDictionary(a => a.Id);
            var polos = (await _poloRepository.GetAllAsync()).ToDictionary(p => p.Id, p => p.Nome);
            foreach (var dto in dtos)
                PreencherAluno(dto, alunos, polos);

            return dtos;
        }

        public async Task<AtletaDTO> Obter(long id)
        {
            var atleta = await _repository.ObterComTudoAsync(id);
            if (atleta == null) return null;

            var dto = _mapper.Map<AtletaDTO>(atleta);
            var alunos = (await _alunoRepository.GetAllAsync()).ToDictionary(a => a.Id);
            var polos = (await _poloRepository.GetAllAsync()).ToDictionary(p => p.Id, p => p.Nome);
            PreencherAluno(dto, alunos, polos);
            return dto;
        }

        public async Task<AtletaDTO> Criar(long alunoId)
        {
            var aluno = await _alunoRepository.GetByIdAsync(alunoId);
            if (aluno == null)
                throw new DomainException("Aluno não encontrado.");
            if (await _repository.ObterPorAlunoAsync(alunoId) != null)
                throw new DomainException("Este aluno já é um atleta.");

            var atleta = new Atleta
            {
                AlunoId = alunoId,
                Status = (int)StatusAtleta.Ativo,
                DataInclusao = DateTime.Now,
                Ativo = true,
            };
            atleta.Validate();
            var criado = await _repository.CriarAsync(atleta);
            return await Obter(criado.Id);
        }

        public async Task<AtletaDTO> AtualizarPerfil(AtletaDTO dto)
        {
            var atleta = _mapper.Map<Atleta>(dto);
            await _repository.AtualizarPerfilAsync(atleta);
            return await Obter(dto.Id);
        }

        public Task Remover(long id) => _repository.RemoverAsync(id);

        // ── Avaliações ───────────────────────────────────────────────────────

        public async Task<AvaliacaoFisicaDTO> AdicionarAvaliacao(long atletaId, AvaliacaoFisicaDTO dto)
        {
            var avaliacao = _mapper.Map<AvaliacaoFisica>(dto);
            avaliacao.Id = 0;
            avaliacao.AtletaId = atletaId;
            foreach (var i in avaliacao.Indicadores) { i.Id = 0; i.AvaliacaoFisicaId = 0; }
            var salva = await _repository.AdicionarAvaliacaoAsync(avaliacao);
            return _mapper.Map<AvaliacaoFisicaDTO>(salva);
        }

        public Task RemoverAvaliacao(long id) => _repository.RemoverAvaliacaoAsync(id);

        // ── Competições ──────────────────────────────────────────────────────

        public async Task<CompeticaoDTO> AdicionarCompeticao(long atletaId, CompeticaoDTO dto)
        {
            var comp = _mapper.Map<Competicao>(dto);
            comp.Id = 0;
            comp.AtletaId = atletaId;
            var salva = await _repository.AdicionarCompeticaoAsync(comp);
            return _mapper.Map<CompeticaoDTO>(salva);
        }

        public Task RemoverCompeticao(long id) => _repository.RemoverCompeticaoAsync(id);

        // ── Diário ───────────────────────────────────────────────────────────

        public async Task<AnotacaoAtletaDTO> AdicionarAnotacao(long atletaId, string texto, string autor)
        {
            if (string.IsNullOrWhiteSpace(texto))
                throw new DomainException("A anotação não pode ficar vazia.");
            var anotacao = new AnotacaoAtleta
            {
                AtletaId = atletaId,
                Data = DateTime.Now,
                Texto = texto,
                Autor = autor ?? string.Empty,
            };
            var salva = await _repository.AdicionarAnotacaoAsync(anotacao);
            return _mapper.Map<AnotacaoAtletaDTO>(salva);
        }

        public Task RemoverAnotacao(long id) => _repository.RemoverAnotacaoAsync(id);

        // ── Metas ────────────────────────────────────────────────────────────

        public async Task<MetaAtletaDTO> AdicionarMeta(long atletaId, MetaAtletaDTO dto)
        {
            var meta = new MetaAtleta
            {
                AtletaId = atletaId,
                Descricao = dto.Descricao,
                Prazo = dto.Prazo,
                Status = (int)StatusMeta.Aberta,
            };
            if (string.IsNullOrWhiteSpace(meta.Descricao))
                throw new DomainException("Descreva a meta.");
            var salva = await _repository.AdicionarMetaAsync(meta);
            return _mapper.Map<MetaAtletaDTO>(salva);
        }

        public async Task<MetaAtletaDTO> AlterarStatusMeta(long id, int status)
        {
            var meta = await _repository.AtualizarMetaAsync(id, status);
            if (meta == null) throw new DomainException("Meta não encontrada.");
            return _mapper.Map<MetaAtletaDTO>(meta);
        }

        public Task RemoverMeta(long id) => _repository.RemoverMetaAsync(id);

        // ── Auxiliar ─────────────────────────────────────────────────────────

        private static void PreencherAluno(
            AtletaDTO dto,
            Dictionary<long, Aluno> alunos,
            Dictionary<long, string> polos)
        {
            if (alunos.TryGetValue(dto.AlunoId, out var aluno))
            {
                dto.AlunoNome = aluno.Nome;
                dto.Faixa = (int)aluno.Faixa;
                dto.PoloNome = polos.TryGetValue(aluno.PoloId, out var pn) ? pn : "—";
            }
            else
            {
                dto.AlunoNome = "—";
                dto.PoloNome = "—";
            }
        }
    }
}
