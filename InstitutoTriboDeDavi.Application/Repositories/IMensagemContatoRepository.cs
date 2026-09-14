using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IMensagemContatoRepository : IBaseRepository<MensagemContato>
    {
        // Todas as mensagens, da mais recente para a mais antiga.
        Task<List<MensagemContato>> ObterTodasAsync();
        // Quantidade de mensagens não lidas (para o contador no menu).
        Task<int> ContarNaoLidasAsync();
    }
}
