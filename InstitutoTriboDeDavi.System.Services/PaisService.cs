using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class PaisService : IPaisService
    {
        private readonly IMapper _mapper;
        private readonly IPaisRepository _paisRepository;

        public PaisService(IMapper mapper, IPaisRepository paisRepository)
        {
            _mapper = mapper;
            _paisRepository = paisRepository;
        }

        public async Task<PaisDTO> Create(PaisDTO paisDTO)
        {
            var paisExists = await _paisRepository.GetByNome(paisDTO.Nome);

            if (paisExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var pais = _mapper.Map<Pais>(paisDTO);

            var paisCreated = await _paisRepository.CreateAsync(pais);

            return _mapper.Map<PaisDTO>(paisCreated);
        }
        public async Task Delete(long id) 
        {
            await _paisRepository.DeleteAsync(id);
        }

        public async Task<PaisDTO> Get(long id)
        {
            var pais = await _paisRepository.GetByIdAsync(id);

            return _mapper.Map<PaisDTO>(pais);
        }

        public async Task<List<PaisDTO>> GetAll()
        {
            var allPaises = await _paisRepository.GetAllAsync();

            return _mapper.Map<List<PaisDTO>>(allPaises);
        }

        public async Task<PaisDTO> GetByNome(string nome)
        {
            var pais = await _paisRepository.GetByNome(nome);

            return _mapper.Map<PaisDTO>(pais);
        }

        public async Task<List<PaisDTO>> SearchByNome(string nome)
        {
            var allPaises = await _paisRepository.SearchByNome(nome);

            return _mapper.Map<List<PaisDTO>>(allPaises);
        }

        public async Task<PaisDTO> Update(PaisDTO paisDTO)
        {
            var paisExists = await _paisRepository.GetByIdAsync(paisDTO.Id);

            if (paisExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var pais = _mapper.Map<Pais>(paisDTO);

            var paisUpdated = await _paisRepository.UpdateAsync(pais);

            return _mapper.Map<PaisDTO>(paisUpdated);
        }
    }
}
