using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
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
        public async Task<List<FrequenciaDTO>> GetAlunosFaltas(long poloId)
        {
            var allAlunos = await _frequenciaRepository.GetAlunosFaltasQuery(poloId);

            return _mapper.Map<List<FrequenciaDTO>>(allAlunos);
        }
    }
}
