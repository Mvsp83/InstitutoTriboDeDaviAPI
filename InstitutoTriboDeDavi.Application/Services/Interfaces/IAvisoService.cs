using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IAvisoService
    {
        Task<List<AvisoDTO>> GetAll();
        Task<AvisoDTO> Create(AvisoDTO dto, UsuarioDTO usuario);
        Task Delete(long id);
        // Avisos ativos do público-alvo do usuário que ele ainda não marcou ciente.
        Task<List<AvisoDTO>> ObterPendentes(UsuarioDTO usuario);
        Task MarcarCiente(long avisoId, UsuarioDTO usuario);
    }
}
