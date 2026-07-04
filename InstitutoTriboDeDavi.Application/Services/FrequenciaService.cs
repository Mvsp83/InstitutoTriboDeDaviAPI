using AutoMapper;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Queries;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class FrequenciaService : IFrequenciaService
    {
        private readonly IMapper _mapper;
        private readonly IFrequenciaRepository _frequenciaRepository;

        public FrequenciaService(IMapper mapper, IFrequenciaRepository frequenciaRepository)
        {
            _mapper = mapper;
            _frequenciaRepository = frequenciaRepository;
        }
        public async Task<List<FrequenciaDTO>> GetAlunosFaltasAsync(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO.Role == Domain.Enums.UserRole.Administrador)
            {
                var allAlunos = await _frequenciaRepository.GetAlunosFaltasQueryTotal();

                return _mapper.Map<List<FrequenciaDTO>>(allAlunos);
            }

            if (usuarioDTO.Role == Domain.Enums.UserRole.Professor && usuarioDTO.PoloId.HasValue)
            {
                var allAlunos = await _frequenciaRepository.GetAlunosFaltasQuery(usuarioDTO.PoloId.Value);

                return _mapper.Map<List<FrequenciaDTO>>(allAlunos);
            }

            throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
        }
    }
}
