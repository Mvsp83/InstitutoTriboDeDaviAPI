using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class EventoCalendarioService : IEventoCalendarioService
    {
        private readonly IMapper _mapper;
        private readonly IEventoCalendarioRepository _repository;

        public EventoCalendarioService(IMapper mapper, IEventoCalendarioRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<EventoCalendarioDTO>> ObterPorAno(int ano)
        {
            var eventos = await _repository.ObterPorAnoAsync(ano);
            return _mapper.Map<List<EventoCalendarioDTO>>(eventos);
        }

        public async Task<List<int>> ObterAnos()
        {
            return await _repository.ObterAnosAsync();
        }

        public async Task<EventoCalendarioDTO> Create(EventoCalendarioDTO dto)
        {
            var evento = _mapper.Map<EventoCalendario>(dto);
            evento.Validate();
            var criado = await _repository.CreateAsync(evento);
            return _mapper.Map<EventoCalendarioDTO>(criado);
        }

        public async Task<EventoCalendarioDTO> Update(EventoCalendarioDTO dto)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null)
                throw new DomainException("Não existe um evento com o ID informado!");

            var evento = _mapper.Map<EventoCalendario>(dto);
            evento.Validate();
            var atualizado = await _repository.UpdateAsync(evento);
            return _mapper.Map<EventoCalendarioDTO>(atualizado);
        }

        public async Task Delete(long id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<int> CopiarAno(int anoOrigem, int anoDestino)
        {
            if (anoOrigem == anoDestino)
                throw new DomainException("Escolha um ano de destino diferente do de origem.");

            var origem = await _repository.ObterPorAnoAsync(anoOrigem);
            if (origem.Count == 0)
                throw new DomainException("O ano de origem não tem eventos para copiar.");

            var destino = await _repository.ObterPorAnoAsync(anoDestino);
            if (destino.Count > 0)
                throw new DomainException(
                    "O ano de destino já possui eventos. Exclua-os antes de copiar.");

            var delta = anoDestino - anoOrigem;
            var novos = origem.Select(e => new EventoCalendario
            {
                Ano = anoDestino,
                // AddYears trata 29/02 automaticamente (cai em 28/02).
                Data = e.Data.AddYears(delta),
                DataFim = e.DataFim?.AddYears(delta),
                Titulo = e.Titulo,
                Tipo = e.Tipo,
                Descricao = e.Descricao,
                PoloId = e.PoloId
            }).ToList();

            await _repository.CriarVariosAsync(novos);
            return novos.Count;
        }
    }
}
