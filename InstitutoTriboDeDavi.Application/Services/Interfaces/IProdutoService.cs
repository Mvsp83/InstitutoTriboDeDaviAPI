using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<List<ProdutoDTO>> Listar();          // gestão (admin)
        Task<List<ProdutoVitrineDTO>> Vitrine();  // pública (só ativos, sem estoque)
        Task<ProdutoDTO> Obter(long id);
        Task<ProdutoDTO> Criar(ProdutoDTO dto);
        Task<ProdutoDTO> Atualizar(ProdutoDTO dto);
        Task Excluir(long id);
    }
}
