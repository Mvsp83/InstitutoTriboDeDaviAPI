using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class ResponsavelService : IResponsavelService
    {
        private readonly IMapper _mapper;
        private readonly IResponsavelRepository _responsavelRepository;

        public ResponsavelService(IMapper mapper, IResponsavelRepository responsavelRepository)
        {
            _mapper = mapper;
            _responsavelRepository = responsavelRepository;
        }

        public async Task<ResponsavelDTO> Create(ResponsavelDTO responsavelDTO)
        {
            var responsavelExists = await _responsavelRepository.GetByNome(responsavelDTO.Nome);

            if (responsavelExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var responsavel = _mapper.Map<Responsavel>(responsavelDTO);

            var responsavelCreated = await _responsavelRepository.CreateAsync(responsavel);

            return _mapper.Map<ResponsavelDTO>(responsavelCreated);
        }
        public async Task Delete(long id)
        {
            await _responsavelRepository.DeleteAsync(id);
        }

        public async Task<ResponsavelDTO> Get(long id)
        {
            var responsavel = await _responsavelRepository.GetByIdAsync(id);

            return _mapper.Map<ResponsavelDTO>(responsavel);
        }

        public async Task<List<ResponsavelDTO>> GetAll()
        {
            var allResponsaveis = await _responsavelRepository.GetAllAsync();

            return _mapper.Map<List<ResponsavelDTO>>(allResponsaveis);
        }

        public async Task<ResponsavelDTO> GetByNome(string nome)
        {
            var responsavel = await _responsavelRepository.GetByNome(nome);

            return _mapper.Map<ResponsavelDTO>(responsavel);
        }

        public async Task<List<ResponsavelDTO>> SearchByNome(string nome)
        {
            var allResponsaveis = await _responsavelRepository.SearchByNome(nome);

            return _mapper.Map<List<ResponsavelDTO>>(allResponsaveis);
        }

        public async Task<ResponsavelDTO> Update(ResponsavelDTO responsavelDTO)
        {
            var responsavelExists = await _responsavelRepository.GetByIdAsync(responsavelDTO.Id);

            if (responsavelExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var responsavel = _mapper.Map<Responsavel>(responsavelDTO);

            var responsavelUpdated = await _responsavelRepository.UpdateAsync(responsavel);

            return _mapper.Map<ResponsavelDTO>(responsavelUpdated);
        }
    }
}
