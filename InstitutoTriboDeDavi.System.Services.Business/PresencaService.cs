using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;
using InstitutoTriboDeDavi.System.DTO.Business;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;

namespace InstitutoTriboDeDavi.System.Services.Business
{
    public class PresencaService : IPresencaService
    {
        private readonly IMapper _mapper;
        private readonly IPresencaRepository _presencaRepository;

        public PresencaService(IMapper mapper, IPresencaRepository presencaRepository)
        {
            _mapper = mapper;
            _presencaRepository = presencaRepository;
        }

        public async Task<PresencaDTO> Create(PresencaDTO presencaDTO)
        {
            var presenca = _mapper.Map<Presenca>(presencaDTO);

            var presencaCreated = await _presencaRepository.CreateAsync(presenca);

            return _mapper.Map<PresencaDTO>(presencaCreated);
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
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var presenca = _mapper.Map<Presenca>(presencaDTO);

            var presencaUpdated = await _presencaRepository.UpdateAsync(presenca);

            return _mapper.Map<PresencaDTO>(presencaUpdated);
        }

        public async Task<List<PresencaDTO>> CreateBatch(IEnumerable<PresencaDTO> presencasDTO)
        {
            var presencas = new List<PresencaDTO>();

            foreach (var presencaDTO in presencasDTO)
            {
                var presencaCreated = await Create(presencaDTO);
                presencas.Add(presencaCreated);
            }

            return presencas;
        }

        public async Task<List<PresencaDTO>> GetPresencasPorAula(long aulaId)
        {
            var presencas =  await _presencaRepository.GetPresencasPorAula(aulaId);

            return _mapper.Map<List<PresencaDTO>>(presencas);
        }

    }
}
