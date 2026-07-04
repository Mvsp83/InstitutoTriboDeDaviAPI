using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Business;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IAulaService
    {
        Task<AulaDTO> Create(AulaDTO aulaDTO);
        Task<AulaDTO> Update(AulaDTO aulaDTO);
        Task Delete(long id);
        Task<AulaDTO> Get(long id);
        Task<List<AulaDTO>> GetAll();
        Task<List<AulaDTO>> ObterAulasTurmaAsync(UsuarioDTO usuario, IEnumerable<int> turmas);
    }
}
