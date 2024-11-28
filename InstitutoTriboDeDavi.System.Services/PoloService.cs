using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class PoloService : IPoloService
    {
        private readonly IMapper _mapper;
        private readonly IPoloRepository _poloRepository;

        public PoloService(IMapper mapper, IPoloRepository poloRepository)
        {
            _mapper = mapper;
            _poloRepository = poloRepository;
        }
        public async Task<PoloDTO> Create(PoloDTO poloDTO)
        {
            var poloExists = await _poloRepository.GetByNome(poloDTO.Nome);

            if (poloExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var polo = _mapper.Map<Polo>(poloDTO);

            var poloCreated = await _poloRepository.CreateAsync(polo);

            return _mapper.Map<PoloDTO>(poloCreated);
        }

        public async Task Delete(long id)
        {
            await _poloRepository.DeleteAsync(id);
        }

        public async Task<PoloDTO> Get(long id)
        {
            var polo = await _poloRepository.GetByIdAsync(id);

            return _mapper.Map<PoloDTO>(polo);
        }

        public async Task<List<PoloDTO>> GetAll()
        {
            var allPolos = await _poloRepository.GetAllAsync();

            return _mapper.Map<List<PoloDTO>>(allPolos);
        }

        public async Task<PoloDTO> GetByNome(string nome)
        {
            var polo = await _poloRepository.GetByNome(nome);

            return _mapper.Map<PoloDTO>(polo);
        }

        public async Task<List<PoloDTO>> SearchByNome(string nome)
        {
            var allPolos = await _poloRepository.SearchByNome(nome);

            return _mapper.Map<List<PoloDTO>>(allPolos);
        }

        public async Task<PoloDTO> Update(PoloDTO poloDTO)
        {
            var poloExists = await _poloRepository.GetByIdAsync(poloDTO.Id);

            if (poloExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var polo = _mapper.Map<Polo>(poloDTO);

            var poloUpdated = await _poloRepository.UpdateAsync(polo);

            return _mapper.Map<PoloDTO>(poloUpdated);
        }
    }
}
