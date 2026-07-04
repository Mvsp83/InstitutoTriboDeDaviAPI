using AutoMapper;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
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
                throw new DomainException("Já existe um registro com o mesmo Nome informado!");
            }

            var polo = _mapper.Map<Polo>(poloDTO);

            polo.Validate();

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
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            var polo = _mapper.Map<Polo>(poloDTO);

            polo.Validate();

            var poloUpdated = await _poloRepository.UpdateAsync(polo);

            return _mapper.Map<PoloDTO>(poloUpdated);
        }

        public async Task<List<PoloDTO>> ObterPolosAsync(UsuarioDTO usuarioDTO, List<int> turmas)
        {
            var listaTodos = await _poloRepository.ObterTodosAsync();

            return _mapper.Map<List<PoloDTO>>(listaTodos);
        }
    }
}
