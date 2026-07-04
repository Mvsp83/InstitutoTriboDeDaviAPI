using AutoMapper;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AulaService : IAulaService
    {
        private readonly IMapper _mapper;
        private readonly IAulaRepository _aulaRepository;

        public AulaService(IMapper mapper, IAulaRepository aulaRepository)
        {
            _mapper = mapper;
            _aulaRepository = aulaRepository;
        }

        public async Task<AulaDTO> Create(AulaDTO aulaDTO)
        {
            var aula = _mapper.Map<Aula>(aulaDTO);

            aula.Validate();

            var aulaCreated = await _aulaRepository.CreateAsync(aula);

            return _mapper.Map<AulaDTO>(aulaCreated);
        }

        public async Task<AulaDTO> Get(long id)
        {
            var aula = await _aulaRepository.GetByIdAsync(id);

            return _mapper.Map<AulaDTO>(aula);
        }

        public async Task Delete(long id)
        {
            await _aulaRepository.DeleteAsync(id);
        }

        public async Task<List<AulaDTO>> GetAll()
        {
            var allAulas = await _aulaRepository.GetAllAsync();

            return _mapper.Map<List<AulaDTO>>(allAulas);
        }

        public async Task<AulaDTO> Update(AulaDTO aulaDTO)
        {
            var aulaExists = await _aulaRepository.GetByIdAsync(aulaDTO.Id);

            if (aulaExists == null)
            {
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            var aula = _mapper.Map<Aula>(aulaDTO);

            aula.Validate();

            var aulaUpdated = await _aulaRepository.UpdateAsync(aula);

            return _mapper.Map<AulaDTO>(aulaUpdated);
        }

        public async Task<List<AulaDTO>> ObterAulasTurmaAsync(UsuarioDTO usuarioDTO, IEnumerable<int> turmas)
        {
            if (usuarioDTO.Role == UserRole.Administrador)
            {
                var listaTodos = await _aulaRepository.ObterTodosAsync();

                return _mapper.Map<List<AulaDTO>>(listaTodos);
            }

            if (usuarioDTO.Role == UserRole.Professor && usuarioDTO.PoloId.HasValue)
            {
                var listaPorPolo = await _aulaRepository.ObterPorPoloTurmaAsync(usuarioDTO.PoloId.Value, turmas);

                return _mapper.Map<List<AulaDTO>>(listaPorPolo);
            }

            throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
        }
    }
}
