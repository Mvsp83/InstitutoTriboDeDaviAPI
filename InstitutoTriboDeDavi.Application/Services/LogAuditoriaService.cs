using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class LogAuditoriaService : ILogAuditoriaService
    {
        private readonly IMapper _mapper;
        private readonly ILogAuditoriaRepository _repository;

        public LogAuditoriaService(IMapper mapper, ILogAuditoriaRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<LogAuditoriaDTO>> Listar(string entidade, string usuario, int limite)
        {
            var logs = await _repository.ListarAsync(entidade, usuario, limite);
            return _mapper.Map<List<LogAuditoriaDTO>>(logs);
        }
    }
}
