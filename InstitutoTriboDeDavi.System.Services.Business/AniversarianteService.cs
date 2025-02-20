using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Queries;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;

namespace InstitutoTriboDeDavi.System.Services.Business
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
