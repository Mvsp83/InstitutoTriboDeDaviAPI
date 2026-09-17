using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IRecadoService
    {
        // Mural visível a quem está logado (vigentes).
        Task<List<RecadoDTO>> ListarVigentes();
        // Gestão da equipe (todos, inclui expirados/inativos).
        Task<List<RecadoDTO>> ListarTodos();
        Task<RecadoDTO> Obter(long id);
        Task<RecadoDTO> Create(RecadoDTO dto, UsuarioDTO usuario);
        // Criação pelo portal (aluno/responsável): nasce pendente de aprovação.
        Task<RecadoDTO> CriarPeloPortal(RecadoDTO dto, string quem, string nome, long? poloId);
        Task Aprovar(long id);
        Task<RecadoDTO> Update(RecadoDTO dto, UsuarioDTO usuario);
        Task Delete(long id, UsuarioDTO usuario);

        // Denúncias (moderação): qualquer logado denuncia; a equipe lista e resolve.
        Task Denunciar(long recadoId, string motivo, string quem);
        Task<List<DenunciaRecadoDTO>> ListarDenunciasPendentes();
        Task ResolverDenuncia(long denunciaId, string login);
    }
}
