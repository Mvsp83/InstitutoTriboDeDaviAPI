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
            var dtos = _mapper.Map<List<BemPatrimonialDTO>>(bens);

            // Preenche as alocações em aberto por bem (para a disponibilidade).
            var abertosPorBem = await _emprestimoRepository.ContarAbertosPorBemAsync();
            foreach (var dto in dtos)
                dto.AlocadosAbertos = abertosPorBem.TryGetValue(dto.Id, out var n) ? n : 0;

            return dtos;
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

            // Disponibilidade por estoque: uma unidade por alocação em aberto.
            var doBem = await _emprestimoRepository.ListarPorBemAsync(dto.BemPatrimonialId);
            var abertos = doBem.Count(e => e.DataDevolucao == null);
            if (abertos >= bem.Quantidade)
                throw new DomainException("Não há unidade disponível deste item para alocar.");

            var emprestimo = new EmprestimoBem
            {
                BemPatrimonialId = dto.BemPatrimonialId,
                AlunoId = dto.AlunoId.HasValue && dto.AlunoId.Value > 0 ? dto.AlunoId : null,
                PoloId = dto.PoloId.HasValue && dto.PoloId.Value > 0 ? dto.PoloId : null,
                DataEmprestimo = DateTime.Now,
                Observacao = dto.Observacao ?? string.Empty,
                RegistradoPor = registrador ?? string.Empty,
            };
            emprestimo.Validate();
            var criado = await _emprestimoRepository.CreateAsync(emprestimo);
            return _mapper.Map<EmprestimoBemDTO>(criado);
        }

        public async Task<EmprestimoBemDTO> Devolver(long emprestimoId, string registrador)
        {
            var emprestimo = await _emprestimoRepository.GetByIdAsync(emprestimoId);
            if (emprestimo == null)
                throw new DomainException("Empréstimo não encontrado.");
            if (emprestimo.DataDevolucao != null)
                throw new DomainException("Este empréstimo já foi devolvido.");

            emprestimo.DataDevolucao = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(registrador))
                emprestimo.RegistradoPor = registrador;
            var atualizado = await _emprestimoRepository.UpdateAsync(emprestimo);

            return _mapper.Map<EmprestimoBemDTO>(atualizado);
        }

        public async Task<List<EmprestimoBemDTO>> HistoricoPorBem(long bemId)
        {
            var lista = await _emprestimoRepository.ListarPorBemAsync(bemId);
            return _mapper.Map<List<EmprestimoBemDTO>>(lista);
        }

        public async Task<List<EmprestimoBemDTO>> HistoricoPorAluno(long alunoId)
        {
            var lista = await _emprestimoRepository.ListarPorAlunoAsync(alunoId);
            return _mapper.Map<List<EmprestimoBemDTO>>(lista);
        }

        public async Task<List<EmprestimoBemDTO>> HistoricoPorPolo(long poloId)
        {
            var lista = await _emprestimoRepository.ListarPorPoloAsync(poloId);
            return _mapper.Map<List<EmprestimoBemDTO>>(lista);
        }
    }
}
