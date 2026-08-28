using System.IO;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    // Storage das fotos no banco (padrão atual do projeto, igual ao dos
    // documentos). O "fileId" é o Id do registro FotoArquivo. Para migrar ao
    // Google Drive/bucket, basta registrar GoogleDriveFotoStorage no Startup.
    public class BancoFotoStorage : IFotoStorage
    {
        private readonly IFotoArquivoRepository _repository;

        public BancoFotoStorage(IFotoArquivoRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> UploadAsync(string nomeArquivo, string contentType, Stream conteudo)
        {
            using var ms = new MemoryStream();
            await conteudo.CopyToAsync(ms);

            var arquivo = new FotoArquivo
            {
                Conteudo = ms.ToArray(),
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "image/jpeg" : contentType,
            };

            var salvo = await _repository.AdicionarAsync(arquivo);
            return salvo.Id.ToString();
        }

        public async Task<FotoDownload> BaixarAsync(string fileId)
        {
            if (!long.TryParse(fileId, out var id)) return null;

            var arquivo = await _repository.ObterAsync(id);
            if (arquivo?.Conteudo == null) return null;

            return new FotoDownload(new MemoryStream(arquivo.Conteudo), arquivo.ContentType);
        }

        public async Task ExcluirAsync(string fileId)
        {
            if (long.TryParse(fileId, out var id))
                await _repository.ExcluirAsync(id);
        }
    }
}
