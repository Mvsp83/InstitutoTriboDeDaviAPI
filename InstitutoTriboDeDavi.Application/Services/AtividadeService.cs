using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.DTO.Queries;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AtividadeService : IAtividadeService
    {
        private readonly IMapper _mapper;
        private readonly IAtividadeRepository _atividadeRepository;

        public AtividadeService(IMapper mapper, IAtividadeRepository atividadeRepository)
        {
            _mapper = mapper;
            _atividadeRepository = atividadeRepository;
        }

        public async Task<AtividadeDTO> Create(AtividadeDTO atividadeDTO)
        {
            var atividade = _mapper.Map<Atividade>(atividadeDTO);

            atividade.Validate();

            var atividadeCreated = await _atividadeRepository.CreateAsync(atividade);

            return _mapper.Map<AtividadeDTO>(atividadeCreated);
        }

        public async Task<AtividadeDTO> Update(AtividadeDTO atividadeDTO)
        {
            var atividadeExists = await _atividadeRepository.GetByIdAsync(atividadeDTO.Id);

            if (atividadeExists == null)
            {
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            var atividade = _mapper.Map<Atividade>(atividadeDTO);

            atividade.Validate();

            var atividadeUpdated = await _atividadeRepository.UpdateAsync(atividade);

            return _mapper.Map<AtividadeDTO>(atividadeUpdated);
        }

        public async Task<AtividadeDTO> Get(long id)
        {
            var atividade = await _atividadeRepository.GetByIdAsync(id);

            return _mapper.Map<AtividadeDTO>(atividade);
        }

        public async Task<List<AtividadeDTO>> GetAll()
        {
            var allAtividades = await _atividadeRepository.GetAllAsync();

            return _mapper.Map<List<AtividadeDTO>>(allAtividades);
        }

        public async Task Delete(long id)
        {
            await _atividadeRepository.DeleteAsync(id);
        }

        public async Task<List<HistoricoAtividadeDTO>> ObterHistoricoTurmaAsync(long poloId, int turma)
        {
            var historico = await _atividadeRepository.ObterHistoricoTurmaAsync(poloId, turma);

            return _mapper.Map<List<HistoricoAtividadeDTO>>(historico);
        }
    }
}
