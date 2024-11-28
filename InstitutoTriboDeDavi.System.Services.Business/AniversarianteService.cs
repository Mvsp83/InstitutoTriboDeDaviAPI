using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
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

        public async Task<List<AniversarianteDTO>> GetAniversariantesAsync(int mes)
        {
            var aniversariantes = await _aniversarianteRepository.GetAniversariantesAsync(mes);

            return _mapper.Map<List<AniversarianteDTO>>(aniversariantes);
        }
    }
}
