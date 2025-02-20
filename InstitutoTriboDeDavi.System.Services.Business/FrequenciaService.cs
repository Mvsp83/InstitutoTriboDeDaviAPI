using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Queries;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;

namespace InstitutoTriboDeDavi.System.Services.Business
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
