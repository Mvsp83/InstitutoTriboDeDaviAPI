using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Business;

namespace InstitutoTriboDeDavi.System.Services.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDTO> Create(AlunoDTO alunoDTO);
        Task<AlunoDTO> Update(AlunoDTO alunoDTO);
        Task Delete(long id);
        Task<AlunoDTO> Get(long id);
        Task<List<AlunoDTO>> GetAll();
        Task<AlunoDTO> GetByNome(string nome);
        Task<List<AlunoDTO>> SearchByNome(string nome);
        Task<int> GetTotalAlunos();
    }
}
