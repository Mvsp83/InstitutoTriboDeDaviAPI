using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class ModeloDeAulaService : IModeloDeAulaService
    {
        private readonly IMapper _mapper;
        private readonly IModeloDeAulaRepository _modeloRepository;

        public ModeloDeAulaService(IMapper mapper, IModeloDeAulaRepository modeloRepository)
        {
            _mapper = mapper;
            _modeloRepository = modeloRepository;
        }

        public async Task<ModeloDeAulaDTO> Create(ModeloDeAulaDTO modeloDTO)
        {
            var modelo = _mapper.Map<ModeloDeAula>(modeloDTO);

            modelo.Validate();

            var modeloCreated = await _modeloRepository.CreateAsync(modelo);

            return _mapper.Map<ModeloDeAulaDTO>(modeloCreated);
        }

        public async Task<ModeloDeAulaDTO> Update(ModeloDeAulaDTO modeloDTO)
        {
            var modeloExists = await _modeloRepository.GetByIdAsync(modeloDTO.Id);

            if (modeloExists == null)
            {
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            var modelo = _mapper.Map<ModeloDeAula>(modeloDTO);

            modelo.Validate();

            var modeloUpdated = await _modeloRepository.UpdateAsync(modelo);

            return _mapper.Map<ModeloDeAulaDTO>(modeloUpdated);
        }

        public async Task<ModeloDeAulaDTO> Get(long id)
        {
            var modelo = await _modeloRepository.GetByIdAsync(id);

            return _mapper.Map<ModeloDeAulaDTO>(modelo);
        }

        public async Task<List<ModeloDeAulaDTO>> GetAll()
        {
            var allModelos = await _modeloRepository.GetAllAsync();

            return _mapper.Map<List<ModeloDeAulaDTO>>(allModelos);
        }

        public async Task Delete(long id)
        {
            await _modeloRepository.DeleteAsync(id);
        }
    }
}
