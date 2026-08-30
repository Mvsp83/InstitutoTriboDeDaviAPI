using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class CompeticaoEventoService : ICompeticaoEventoService
    {
        private readonly IMapper _mapper;
        private readonly ICompeticaoEventoRepository _repository;
        private readonly IAtletaRepository _atletaRepository;
        private readonly IAlunoRepository _alunoRepository;

        public CompeticaoEventoService(
            IMapper mapper,
            ICompeticaoEventoRepository repository,
            IAtletaRepository atletaRepository,
            IAlunoRepository alunoRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _atletaRepository = atletaRepository;
            _alunoRepository = alunoRepository;
        }

        public async Task<List<CompeticaoEventoDTO>> Listar()
        {
            var eventos = await _repository.ListarAsync();
            var dtos = _mapper.Map<List<CompeticaoEventoDTO>>(eventos);
            for (var i = 0; i < dtos.Count; i++)
            {
                dtos[i].TotalParticipantes = eventos[i].Participacoes.Count;
                dtos[i].Participacoes = new(); // lista leve
            }
            return dtos;
        }

        public async Task<CompeticaoEventoDTO> Obter(long id)
        {
            var evento = await _repository.ObterComParticipacoesAsync(id);
            if (evento == null) return null;
            var dto = _mapper.Map<CompeticaoEventoDTO>(evento);
            dto.TotalParticipantes = dto.Participacoes.Count;
            await PreencherNomes(dto.Participacoes);
            return dto;
        }

        public async Task<CompeticaoEventoDTO> Criar(CompeticaoEventoDTO dto)
        {
            var evento = _mapper.Map<CompeticaoEvento>(dto);
            evento.Id = 0;
            evento.Participacoes = new();
            evento.Validate();
            var criado = await _repository.CriarAsync(evento);
            return await Obter(criado.Id);
        }

        public async Task<CompeticaoEventoDTO> Atualizar(CompeticaoEventoDTO dto)
        {
            var evento = _mapper.Map<CompeticaoEvento>(dto);
            evento.Validate();
            await _repository.AtualizarAsync(evento);
            return await Obter(dto.Id);
        }

        public Task Remover(long id) => _repository.RemoverAsync(id);

        public async Task<ParticipacaoAtletaDTO> AdicionarParticipacao(long eventoId, ParticipacaoAtletaDTO dto)
        {
            var p = _mapper.Map<ParticipacaoAtleta>(dto);
            p.Id = 0;
            p.CompeticaoEventoId = eventoId;
            var salva = await _repository.AdicionarParticipacaoAsync(p);
            var resultado = _mapper.Map<ParticipacaoAtletaDTO>(salva);
            await PreencherNomes(new List<ParticipacaoAtletaDTO> { resultado });
            return resultado;
        }

        public async Task<ParticipacaoAtletaDTO> AtualizarParticipacao(ParticipacaoAtletaDTO dto)
        {
            var p = _mapper.Map<ParticipacaoAtleta>(dto);
            var atual = await _repository.AtualizarParticipacaoAsync(p);
            var resultado = _mapper.Map<ParticipacaoAtletaDTO>(atual);
            await PreencherNomes(new List<ParticipacaoAtletaDTO> { resultado });
            return resultado;
        }

        public Task RemoverParticipacao(long id) => _repository.RemoverParticipacaoAsync(id);

        private async Task PreencherNomes(List<ParticipacaoAtletaDTO> participacoes)
        {
            if (participacoes.Count == 0) return;
            var atletas = (await _atletaRepository.ListarAsync())
                .ToDictionary(a => a.Id, a => a.AlunoId);
            var alunos = (await _alunoRepository.GetAllAsync()).ToDictionary(a => a.Id);
            foreach (var p in participacoes)
            {
                if (atletas.TryGetValue(p.AtletaId, out var alunoId) &&
                    alunos.TryGetValue(alunoId, out var aluno))
                {
                    p.AtletaNome = aluno.Nome;
                    p.Faixa = (int)aluno.Faixa;
                }
                else
                {
                    p.AtletaNome = "—";
                }
            }
        }
    }
}
