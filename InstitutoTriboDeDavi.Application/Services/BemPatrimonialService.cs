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
        private readonly IEmprestimoBemRepository _emprestimoRepository;

        public BemPatrimonialService(
            IMapper mapper,
            IBemPatrimonialRepository repository,
            IEmprestimoBemRepository emprestimoRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _emprestimoRepository = emprestimoRepository;
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

        public async Task<EmprestimoBemDTO> Emprestar(EmprestarBemDTO dto, string registrador)
        {
            var bem = await _repository.GetByIdAsync(dto.BemPatrimonialId);
            if (bem == null)
                throw new DomainException("Bem não encontrado.");

            var aberto = await _emprestimoRepository.ObterAbertoPorBemAsync(dto.BemPatrimonialId);
            if (aberto != null || bem.AlunoId != null)
                throw new DomainException("Este item já está emprestado. Registre a devolução primeiro.");

            var emprestimo = new EmprestimoBem
            {
                BemPatrimonialId = dto.BemPatrimonialId,
                AlunoId = dto.AlunoId,
                DataEmprestimo = DateTime.Now,
                Observacao = dto.Observacao ?? string.Empty,
                RegistradoPor = registrador ?? string.Empty,
            };
            emprestimo.Validate();
            var criado = await _emprestimoRepository.CreateAsync(emprestimo);

            // Mantém o "com quem está" no próprio bem (usado na lista/disponibilidade).
            bem.AlunoId = dto.AlunoId;
            await _repository.UpdateAsync(bem);

            return _mapper.Map<EmprestimoBemDTO>(criado);
        }

        public async Task<EmprestimoBemDTO> Devolver(long bemId, string registrador)
        {
            var bem = await _repository.GetByIdAsync(bemId);
            if (bem == null)
                throw new DomainException("Bem não encontrado.");

            var aberto = await _emprestimoRepository.ObterAbertoPorBemAsync(bemId);
            if (aberto == null)
            {
                // Sem empréstimo em aberto: só garante o bem como disponível.
                if (bem.AlunoId != null)
                {
                    bem.AlunoId = null;
                    await _repository.UpdateAsync(bem);
                }
                throw new DomainException("Não há empréstimo em aberto para este item.");
            }

            aberto.DataDevolucao = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(registrador))
                aberto.RegistradoPor = registrador;
            var atualizado = await _emprestimoRepository.UpdateAsync(aberto);

            bem.AlunoId = null;
            await _repository.UpdateAsync(bem);

            return _mapper.Map<EmprestimoBemDTO>(atualizado);
        }

        public async Task<List<EmprestimoBemDTO>> HistoricoPorBem(long bemId)
        {
            var lista = await _emprestimoRepository.ListarPorBemAsync(bemId);
            return _mapper.Map<List<EmprestimoBemDTO>>(lista);
        }
    }
}
