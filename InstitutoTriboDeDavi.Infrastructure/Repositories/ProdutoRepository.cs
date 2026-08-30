using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        private readonly TriboDeDaviContext _context;

        public ProdutoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Produto>> ListarTodosAsync()
        {
            return await _context.Produtos
                .Include(p => p.Variacoes)
                .AsNoTracking()
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<Produto>> ListarVitrineAsync()
        {
            return await _context.Produtos
                .Where(p => p.Ativo)
                .Include(p => p.Variacoes)
                .AsNoTracking()
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<Produto> ObterComVariacoesAsync(long id)
        {
            return await _context.Produtos
                .Include(p => p.Variacoes)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Produto> AtualizarComVariacoesAsync(Produto produto)
        {
            var existente = await _context.Produtos
                .Include(p => p.Variacoes)
                .FirstOrDefaultAsync(p => p.Id == produto.Id);

            if (existente == null)
                return null;

            existente.Nome = produto.Nome;
            existente.Descricao = produto.Descricao;
            existente.Preco = produto.Preco;
            existente.FormasPagamento = produto.FormasPagamento;
            existente.Informacoes = produto.Informacoes;
            existente.Ativo = produto.Ativo;
            // Só troca a foto quando veio uma nova (evita apagar a atual ao editar).
            if (!string.IsNullOrEmpty(produto.FotoArquivoId))
                existente.FotoArquivoId = produto.FotoArquivoId;

            _context.VariacoesProduto.RemoveRange(existente.Variacoes);
            foreach (var v in produto.Variacoes ?? new List<VariacaoProduto>())
            {
                _context.VariacoesProduto.Add(new VariacaoProduto
                {
                    ProdutoId = existente.Id,
                    Tamanho = v.Tamanho,
                    Cor = v.Cor,
                    Quantidade = v.Quantidade,
                });
            }

            await _context.SaveChangesAsync();
            return existente;
        }
    }
}
