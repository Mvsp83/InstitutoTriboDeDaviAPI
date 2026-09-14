using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IBemPatrimonialService
    {
        Task<List<BemPatrimonialDTO>> GetAll();
        Task<BemPatrimonialDTO> Get(long id);
        Task<BemPatrimonialDTO> Create(BemPatrimonialDTO dto);
        Task<BemPatrimonialDTO> Update(BemPatrimonialDTO dto);
        Task Delete(long id);

        // Alocação/comodato: quimono/faixa a um aluno, tatame a um polo.
        Task<EmprestimoBemDTO> Emprestar(EmprestarBemDTO dto, string registrador);
        // Devolve uma alocação específica (por id do empréstimo).
        Task<EmprestimoBemDTO> Devolver(long emprestimoId, string registrador);
        Task<List<EmprestimoBemDTO>> HistoricoPorBem(long bemId);
        Task<List<EmprestimoBemDTO>> HistoricoPorAluno(long alunoId);
        Task<List<EmprestimoBemDTO>> HistoricoPorPolo(long poloId);
    }
}
