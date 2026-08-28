using System.IO;
using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Armazenamento do binário das fotos de treino (hoje Google Drive, numa
    // pasta própria). Separado de IDocumentoDriveService porque aquele é para
    // documentos contábeis (whitelist e pastas diferentes). Trocar por um bucket
    // depois é só implementar esta interface em outra classe.
    public interface IFotoStorage
    {
        // Sobe o arquivo e devolve o id do storage (fileId).
        Task<string> UploadAsync(string nomeArquivo, string contentType, Stream conteudo);
        Task<FotoDownload> BaixarAsync(string fileId);
        Task ExcluirAsync(string fileId);
    }

    // Conteúdo binário de uma foto baixada do storage.
    public record FotoDownload(Stream Conteudo, string ContentType);
}
