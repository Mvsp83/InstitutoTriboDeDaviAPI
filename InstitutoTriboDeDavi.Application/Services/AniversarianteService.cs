using AutoMapper;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Queries;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AniversarianteService : IAniversarianteService
    {
        private readonly IMapper _mapper;
        private readonly IAniversarianteRepository _aniversarianteRepository;

        public AniversarianteService(IAniversarianteRepository aniversarianteRepository, IMapper mapper)
        {
            _aniversarianteRepository = aniversarianteRepository;
            _mapper = mapper;
        }

        public async Task<List<AniversarianteDTO>> GetAniversariantesAsync(UsuarioDTO usuarioDTO, int mes)
        {
            if (usuarioDTO.Role == Domain.Enums.UserRole.Administrador)
            {
                var aniversariantes = await _aniversarianteRepository.GetAniversariantesAsync(mes);

                return _mapper.Map<List<AniversarianteDTO>>(aniversariantes);
            }

            if (usuarioDTO.Role == Domain.Enums.UserRole.Professor)
            {
                var aniversariantes = await _aniversarianteRepository.GetAniversariantesPorPoloAsync(mes, usuarioDTO.PoloId.Value);

                return _mapper.Map<List<AniversarianteDTO>>(aniversariantes);
            }

            throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
        }
    }
}
