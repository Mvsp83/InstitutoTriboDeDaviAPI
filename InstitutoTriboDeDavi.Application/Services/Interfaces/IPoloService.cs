using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IPoloService
    {
        Task<PoloDTO> Create(PoloDTO poloDTO);
        Task<PoloDTO> Update(PoloDTO poloDTO);
        Task Delete(long id);
        Task<PoloDTO> Get(long id);
        Task<List<PoloDTO>> GetAll();
        Task<PoloDTO> GetByNome(string nome);
        Task<List<PoloDTO>> SearchByNome(string nome);
        Task<List<PoloDTO>> ObterPolosAsync(UsuarioDTO usuarioDTO, List<int> turmas);
        // Polos para a página pública de Informações (com endereço e horários).
        Task<List<PoloPublicoDetalhadoDTO>> ListarPublicos();
    }
}
