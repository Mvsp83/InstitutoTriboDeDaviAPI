using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Business;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IPlanoDeAulaService
    {
        Task<PlanoDeAulaDTO> Create(PlanoDeAulaDTO planoDTO);
        Task<PlanoDeAulaDTO> Update(PlanoDeAulaDTO planoDTO);
        Task<PlanoDeAulaDTO> Get(long id);
        Task<List<PlanoDeAulaDTO>> GetAll();
        Task Delete(long id);
        Task<List<PlanoDeAulaDTO>> ObterPlanosTurmaAsync(UsuarioDTO usuarioDTO, IEnumerable<int> turmas);
        Task<PlanoDeAulaDTO> CriarDeModelo(long modeloId, PlanoDeAulaDTO dadosBase);
        Task<PlanoDeAulaDTO> Clonar(long planoId, DateTime novaDataPrevista);
    }
}
