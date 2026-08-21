using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class BemPatrimonialService : IBemPatrimonialService
    {
        private readonly IMapper _mapper;
        private readonly IBemPatrimonialRepository _repository;

        public BemPatrimonialService(IMapper mapper, IBemPatrimonialRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<BemPatrimonialDTO>> GetAll()
        {
            var bens = await _repository.ObterTodosAsync();
            return _mapper.Map<List<BemPatrimonialDTO>>(bens);
        }

        public async Task<BemPatrimonialDTO> Get(long id)
        {
            var bem = await _repository.GetByIdAsync(id);
            return _mapper.Map<BemPatrimonialDTO>(bem);
        }

        public async Task<BemPatrimonialDTO> Create(BemPatrimonialDTO dto)
        {
            var bem = _mapper.Map<BemPatrimonial>(dto);
            bem.Validate();
            var criado = await _repository.CreateAsync(bem);
            return _mapper.Map<BemPatrimonialDTO>(criado);
        }

        public async Task<BemPatrimonialDTO> Update(BemPatrimonialDTO dto)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null)
                throw new DomainException("Bem não encontrado.");

            var bem = _mapper.Map<BemPatrimonial>(dto);
            bem.Validate();
            var atualizado = await _repository.UpdateAsync(bem);
            return _mapper.Map<BemPatrimonialDTO>(atualizado);
        }

        public async Task Delete(long id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
