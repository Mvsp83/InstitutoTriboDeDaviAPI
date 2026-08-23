using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IDocumentoArquivoRepository
    {
        // Metadados dos documentos de uma categoria (SEM o binário Conteudo).
        Task<List<DocumentoArquivo>> ListarPorCategoriaAsync(int categoria);

        // Documento completo, com o binário, para download.
        Task<DocumentoArquivo> ObterAsync(long id);

        Task<DocumentoArquivo> CriarAsync(DocumentoArquivo documento);

        // Remove; retorna false se não existia.
        Task<bool> ExcluirAsync(long id);
    }
}
