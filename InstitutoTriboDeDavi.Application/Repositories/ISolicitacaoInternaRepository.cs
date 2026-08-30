using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface ISolicitacaoInternaRepository : IBaseRepository<SolicitacaoInterna>
    {
        // Todas (admin/supervisor), sem carregar a conversa — para a listagem.
        Task<List<SolicitacaoInterna>> ListarTodasAsync();

        // Só as que o usuário abriu ou recebeu — para a listagem do professor.
        Task<List<SolicitacaoInterna>> ListarDoUsuarioAsync(string login);

        // Uma solicitação com toda a conversa (detalhe).
        Task<SolicitacaoInterna> ObterComMensagensAsync(long id);

        // Acrescenta uma mensagem à conversa.
        Task AdicionarMensagemAsync(MensagemSolicitacao mensagem);

        // Contagem de não-resolvidas visíveis ao usuário (badge). login/role via serviço.
        Task<int> ContarNaoResolvidasTodasAsync();
        Task<int> ContarNaoResolvidasDoUsuarioAsync(string login);
    }
}
