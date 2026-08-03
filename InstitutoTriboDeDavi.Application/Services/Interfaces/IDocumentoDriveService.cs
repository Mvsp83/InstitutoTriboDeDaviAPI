using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Armazenamento de documentos contábeis (DRE, Balanço, Relatório de
    // Atividades) no Google Drive do instituto. Sem banco: a listagem é lida
    // diretamente das subpastas do Drive.
    public interface IDocumentoDriveService
    {
        Task<IReadOnlyList<DocumentoArquivoDTO>> ListarAsync(CategoriaDocumento categoria);

        Task<DocumentoArquivoDTO> UploadAsync(
            CategoriaDocumento categoria,
            string nomeArquivo,
            string contentType,
            Stream conteudo);

        Task<DocumentoDownload?> BaixarAsync(string fileId);

        Task<bool> ExcluirAsync(string fileId);
    }

    // Conteúdo binário de um arquivo baixado do Drive, junto de seus metadados.
    public record DocumentoDownload(Stream Conteudo, string ContentType, string Nome);
}
