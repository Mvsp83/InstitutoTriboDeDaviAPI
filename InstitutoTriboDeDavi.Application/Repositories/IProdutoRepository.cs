using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        // Todos, com variações (gestão admin).
        Task<List<Produto>> ListarTodosAsync();
        // Só os ativos, com variações (vitrine pública).
        Task<List<Produto>> ListarVitrineAsync();
        // Um produto com as variações.
        Task<Produto> ObterComVariacoesAsync(long id);
        // Atualiza o produto e substitui as variações.
        Task<Produto> AtualizarComVariacoesAsync(Produto produto);
    }
}
