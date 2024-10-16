using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class EnderecoService : IEnderecoService
    {
        private readonly IMapper _mapper;
        private readonly IEnderecoRepository _enderecoRepository;

        public EnderecoService(IMapper mapper, IEnderecoRepository enderecoRepository)
        {
            _mapper = mapper;
            _enderecoRepository = enderecoRepository;
        }

        public async Task<EnderecoDTO> Create(EnderecoDTO enderecoDTO)
        {
            var enderecoExists = await _enderecoRepository.GetByNome(enderecoDTO.Logradouro);

            if (enderecoExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var endereco = _mapper.Map<Endereco>(enderecoDTO);

            var enderecoCreated = await _enderecoRepository.CreateAsync(endereco);

            return _mapper.Map<EnderecoDTO>(enderecoCreated);
        }
        public async Task Delete(long id)
        {
            await _enderecoRepository.DeleteAsync(id);
        }

        public async Task<EnderecoDTO> Get(long id)
        {
            var endereco = await _enderecoRepository.GetByIdAsync(id);

            return _mapper.Map<EnderecoDTO>(endereco);
        }

        public async Task<List<EnderecoDTO>> GetAll()
        {
            var allEnderecos = await _enderecoRepository.GetAllAsync();

            return _mapper.Map<List<EnderecoDTO>>(allEnderecos);
        }

        public async Task<EnderecoDTO> GetByNome(string nome)
        {
            var endereco = await _enderecoRepository.GetByNome(nome);

            return _mapper.Map<EnderecoDTO>(endereco);
        }

        public async Task<List<EnderecoDTO>> SearchByNome(string nome)
        {
            var allEnderecos = await _enderecoRepository.SearchByNome(nome);

            return _mapper.Map<List<EnderecoDTO>>(allEnderecos);
        }

        public async Task<EnderecoDTO> Update(EnderecoDTO enderecoDTO)
        {
            var enderecoExists = await _enderecoRepository.GetByIdAsync(enderecoDTO.Id);

            if (enderecoExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var endereco = _mapper.Map<Endereco>(enderecoDTO);

            var enderecoUpdated = await _enderecoRepository.UpdateAsync(endereco);

            return _mapper.Map<EnderecoDTO>(enderecoUpdated);
        }
    }
}
