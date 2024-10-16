using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class BairroService : IBairroService
    {
        private readonly IMapper _mapper;
        private readonly IBairroRepository _bairroRepository;

        public BairroService(IMapper mapper, IBairroRepository bairroRepository)
        {
            _mapper = mapper;
            _bairroRepository = bairroRepository;
        }

        public async Task<BairroDTO> Create(BairroDTO bairroDTO)
        {
            var bairroExists = await _bairroRepository.GetByNome(bairroDTO.Nome);

            if (bairroExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var bairro = _mapper.Map<Bairro>(bairroDTO);

            var bairroCreated = await _bairroRepository.CreateAsync(bairro);

            return _mapper.Map<BairroDTO>(bairroCreated);
        }
        public async Task Delete(long id)
        {
            await _bairroRepository.DeleteAsync(id);
        }

        public async Task<BairroDTO> Get(long id)
        {
            var bairro = await _bairroRepository.GetByIdAsync(id);

            return _mapper.Map<BairroDTO>(bairro);
        }

        public async Task<List<BairroDTO>> GetAll()
        {
            var allBairros = await _bairroRepository.GetAllAsync();

            return _mapper.Map<List<BairroDTO>>(allBairros);
        }

        public async Task<BairroDTO> GetByNome(string nome)
        {
            var bairro = await _bairroRepository.GetByNome(nome);

            return _mapper.Map<BairroDTO>(bairro);
        }

        public async Task<List<BairroDTO>> SearchByNome(string nome)
        {
            var allBairros = await _bairroRepository.SearchByNome(nome);

            return _mapper.Map<List<BairroDTO>>(allBairros);
        }

        public async Task<BairroDTO> Update(BairroDTO bairroDTO)
        {
            var bairroExists = await _bairroRepository.GetByIdAsync(bairroDTO.Id);

            if (bairroExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var bairro = _mapper.Map<Bairro>(bairroDTO);

            var bairroUpdated = await _bairroRepository.UpdateAsync(bairro);

            return _mapper.Map<BairroDTO>(bairroUpdated);
        }
    }
}
