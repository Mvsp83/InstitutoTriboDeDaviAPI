using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface ISolicitacaoInternaService
    {
        // Caixa de entrada do usuário: admin/supervisor veem tudo; professor vê
        // só o que abriu ou recebeu.
        Task<List<SolicitacaoInternaDTO>> Listar(UsuarioDTO usuario);

        // Detalhe com a conversa. Null se não existe ou o usuário não pode ver.
        Task<SolicitacaoInternaDTO> Obter(long id, UsuarioDTO usuario);

        Task<SolicitacaoInternaDTO> Criar(CriarSolicitacaoDTO dados, UsuarioDTO usuario);

        // Adiciona uma resposta à conversa. Null se não pode responder.
        Task<SolicitacaoInternaDTO> Responder(long id, string texto, UsuarioDTO usuario);

        // Muda o status (Aberta/EmAndamento/Resolvida). Null se não autorizado.
        Task<SolicitacaoInternaDTO> AlterarStatus(long id, int status, UsuarioDTO usuario);

        // Total de não-resolvidas visíveis ao usuário (badge).
        Task<int> ContarNaoResolvidas(UsuarioDTO usuario);
    }
}
