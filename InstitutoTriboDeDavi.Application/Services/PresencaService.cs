using AutoMapper;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class PresencaService : IPresencaService
    {
        private readonly IMapper _mapper;
        private readonly IPresencaRepository _presencaRepository;
        private readonly IAulaRepository _aulaRepository;

        public PresencaService(IMapper mapper, IPresencaRepository presencaRepository, IAulaRepository aulaRepository)
        {
            _mapper = mapper;
            _presencaRepository = presencaRepository;
            _aulaRepository = aulaRepository;
        }

        public async Task<PresencaDTO> Create(PresencaDTO presencaDTO)
        {
            var presenca = _mapper.Map<Presenca>(presencaDTO);

            presenca.Validate();

            var presencaCreated = await _presencaRepository.CreateAsync(presenca);
            return _mapper.Map<PresencaDTO>(presencaCreated);
        }

        public async Task<List<PresencaDTO>> CreateBatch(IEnumerable<PresencaDTO> presencasDTO)
        {
            var presencas = _mapper.Map<List<Presenca>>(presencasDTO);

            if (presencas.Count == 0)
                throw new DomainException("Nenhuma presença foi informada.");

            var aulaIds = presencas.Select(p => p.AulaId).Distinct().ToList();

            if (aulaIds.Count > 1)
                throw new DomainException("Todas as presenças do lote devem pertencer à mesma aula.");

            foreach (var presenca in presencas)
                presenca.Validate();

            var aula = await _aulaRepository.GetByIdAsync(aulaIds[0]);

            if (aula == null)
                throw new DomainException("Não existe uma aula com o ID informado!");

            // Trava anti-duplicação: reenvio do mesmo lote não grava duas vezes
            if (aula.PresencaSalva)
                throw new DomainException("As presenças desta aula já foram salvas.");

            var presencasCriadas = await _presencaRepository.CreateBatchComAulaAsync(presencas, aula.Id);
            return _mapper.Map<List<PresencaDTO>>(presencasCriadas);
        }

        public async Task<PresencaDTO> Get(long id)
        {
            var presenca = await _presencaRepository.GetByIdAsync(id);
            return _mapper.Map<PresencaDTO>(presenca);
        }

        public async Task Delete(long id)
        {
            await _presencaRepository.DeleteAsync(id);
        }

        public async Task<List<PresencaDTO>> GetAll()
        {
            var allPresencas = await _presencaRepository.GetAllAsync();
            return _mapper.Map<List<PresencaDTO>>(allPresencas);
        }

        public async Task<PresencaDTO> Update(PresencaDTO presencaDTO)
        {
            var presencaExists = await _presencaRepository.GetByIdAsync(presencaDTO.Id);

            if (presencaExists == null)
                throw new DomainException("Não existe um registro com o ID informado!");

            var presenca = _mapper.Map<Presenca>(presencaDTO);

            presenca.Validate();

            var presencaUpdated = await _presencaRepository.UpdateAsync(presenca);
            return _mapper.Map<PresencaDTO>(presencaUpdated);
        }

        public async Task<List<PresencaDTO>> GetPresencasPorAula(long aulaId)
        {
            var presencas = await _presencaRepository.GetPresencasPorAula(aulaId);
            return _mapper.Map<List<PresencaDTO>>(presencas);
        }
    }
}
