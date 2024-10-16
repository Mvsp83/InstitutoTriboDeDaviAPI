using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class CidadeService : ICidadeService
    {
        private readonly IMapper _mapper;
        private readonly ICidadeRepository _cidadeRepository;

        public CidadeService(IMapper mapper, ICidadeRepository cidadeRepository)
        {
            _mapper = mapper;
            _cidadeRepository = cidadeRepository;
        }

        public async Task<CidadeDTO> Create(CidadeDTO cidadeDTO)
        {
            var cidadeExists = await _cidadeRepository.GetByNome(cidadeDTO.Nome);

            if (cidadeExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var cidade = _mapper.Map<Cidade>(cidadeDTO);

            var cidadeCreated = await _cidadeRepository.CreateAsync(cidade);

            return _mapper.Map<CidadeDTO>(cidadeCreated);
        }
        public async Task Delete(long id)
        {
            await _cidadeRepository.DeleteAsync(id);
        }

        public async Task<CidadeDTO> Get(long id)
        {
            var cidade = await _cidadeRepository.GetByIdAsync(id);

            return _mapper.Map<CidadeDTO>(cidade);
        }

        public async Task<List<CidadeDTO>> GetAll()
        {
            var allCidades = await _cidadeRepository.GetAllAsync();

            return _mapper.Map<List<CidadeDTO>>(allCidades);
        }

        public async Task<CidadeDTO> GetByNome(string nome)
        {
            var cidade = await _cidadeRepository.GetByNome(nome);

            return _mapper.Map<CidadeDTO>(cidade);
        }

        public async Task<List<CidadeDTO>> SearchByNome(string nome)
        {
            var allCidades = await _cidadeRepository.SearchByNome(nome);

            return _mapper.Map<List<CidadeDTO>>(allCidades);
        }

        public async Task<CidadeDTO> Update(CidadeDTO cidadeDTO)
        {
            var cidadeExists = await _cidadeRepository.GetByIdAsync(cidadeDTO.Id);

            if (cidadeExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var cidade = _mapper.Map<Cidade>(cidadeDTO);

            var cidadeUpdated = await _cidadeRepository.UpdateAsync(cidade);

            return _mapper.Map<CidadeDTO>(cidadeUpdated);
        }
    }
}
