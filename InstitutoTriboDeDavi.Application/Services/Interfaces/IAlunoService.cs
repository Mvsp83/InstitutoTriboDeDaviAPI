using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
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
        Task<List<AlunoDTO>> ObterAlunosPorTurmaAsync(UsuarioDTO usuario, List<int> turmas);
        Task<List<AlunoPendenteDTO>> ObterAlunosPendentesAsync(UsuarioDTO usuario);
        Task<AlunoDTO> AtribuirTurmaAsync(AtribuirTurmaDTO dto);
    }
}
