using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.Services
{
    // Armazenamento de documentos no PRÓPRIO BANCO (sem Google Drive). Implementa
    // a mesma interface do serviço do Drive, então controller e front não mudam:
    // o "fileId" que eles usam é o Id (long) da linha, como string.
    public class DocumentoBancoService : IDocumentoDriveService
    {
        private readonly IDocumentoArquivoRepository _repository;

        public DocumentoBancoService(IDocumentoArquivoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<DocumentoArquivoDTO>> ListarAsync(CategoriaDocumento categoria)
        {
            var docs = await _repository.ListarPorCategoriaAsync((int)categoria);
            return docs.Select(ToDTO).ToList();
        }

        public async Task<DocumentoArquivoDTO> UploadAsync(
            CategoriaDocumento categoria,
            string nomeArquivo,
            string contentType,
            Stream conteudo)
        {
            using var ms = new MemoryStream();
            await conteudo.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var criado = await _repository.CriarAsync(new DocumentoArquivo
            {
                Categoria = (int)categoria,
                Nome = string.IsNullOrWhiteSpace(nomeArquivo) ? "documento" : nomeArquivo,
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
                TamanhoBytes = bytes.LongLength,
                Conteudo = bytes,
                DataCriacao = DateTime.Now,
            });

            return ToDTO(criado);
        }

        public async Task<DocumentoDownload?> BaixarAsync(string fileId)
        {
            if (!long.TryParse(fileId, out var id))
                return null;

            var doc = await _repository.ObterAsync(id);
            if (doc == null || doc.Conteudo == null)
                return null;

            return new DocumentoDownload(new MemoryStream(doc.Conteudo), doc.ContentType, doc.Nome);
        }

        public async Task<bool> ExcluirAsync(string fileId)
        {
            if (!long.TryParse(fileId, out var id))
                return false;
            return await _repository.ExcluirAsync(id);
        }

        private static DocumentoArquivoDTO ToDTO(DocumentoArquivo d) => new()
        {
            Id = d.Id.ToString(),
            Nome = d.Nome,
            TamanhoBytes = d.TamanhoBytes,
            DataCriacao = d.DataCriacao,
            MimeType = d.ContentType,
        };
    }
}
