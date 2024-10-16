using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class EstadoService : IEstadoService
    {
        private readonly IMapper _mapper;
        private readonly IEstadoRepository _estadoRepository;

        public EstadoService(IMapper mapper, IEstadoRepository estadoRepository)
        {
            _mapper = mapper;
            _estadoRepository = estadoRepository;
        }

        public async Task<EstadoDTO> Create(EstadoDTO estadoDTO)
        {
            var estadoExists = await _estadoRepository.GetByNome(estadoDTO.Nome);

            if (estadoExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var estado = _mapper.Map<Estado>(estadoDTO);

            var estadoCreated = await _estadoRepository.CreateAsync(estado);

            return _mapper.Map<EstadoDTO>(estadoCreated);
        }
        public async Task Delete(long id)
        {
            await _estadoRepository.DeleteAsync(id);
        }

        public async Task<EstadoDTO> Get(long id)
        {
            var estado = await _estadoRepository.GetByIdAsync(id);

            return _mapper.Map<EstadoDTO>(estado);
        }

        public async Task<List<EstadoDTO>> GetAll()
        {
            var allEstados = await _estadoRepository.GetAllAsync();

            return _mapper.Map<List<EstadoDTO>>(allEstados);
        }

        public async Task<EstadoDTO> GetByNome(string nome)
        {
            var estado = await _estadoRepository.GetByNome(nome);

            return _mapper.Map<EstadoDTO>(estado);
        }

        public async Task<List<EstadoDTO>> SearchByNome(string nome)
        {
            var allEstados = await _estadoRepository.SearchByNome(nome);

            return _mapper.Map<List<EstadoDTO>>(allEstados);
        }

        public async Task<EstadoDTO> Update(EstadoDTO estadoDTO)
        {
            var estadoExists = await _estadoRepository.GetByIdAsync(estadoDTO.Id);

            if (estadoExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var estado = _mapper.Map<Estado>(estadoDTO);

            var estadoUpdated = await _estadoRepository.UpdateAsync(estado);

            return _mapper.Map<EstadoDTO>(estadoUpdated);
        }
    }
}
