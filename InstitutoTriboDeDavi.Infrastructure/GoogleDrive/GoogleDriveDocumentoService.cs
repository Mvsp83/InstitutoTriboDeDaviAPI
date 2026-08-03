using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace InstitutoTriboDeDavi.Infrastructure.GoogleDrive
{
    public class GoogleDriveDocumentoService : IDocumentoDriveService
    {
        private const string MimeTypePasta = "application/vnd.google-apps.folder";

        // Whitelist autoritativa de extensões (espelha o portal antigo).
        private static readonly HashSet<string> ExtensoesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        {
            ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".ppt", ".pptx", ".txt"
        };

        private readonly GoogleDriveConfig _config;
        private readonly ILogger<GoogleDriveDocumentoService> _logger;

        private DriveService? _driveService;
        // Cache dos IDs de subpasta por categoria dentro do escopo (evita
        // uma busca a cada chamada).
        private readonly Dictionary<CategoriaDocumento, string> _subpastas = new();

        public GoogleDriveDocumentoService(
            IOptions<GoogleDriveConfig> config,
            ILogger<GoogleDriveDocumentoService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task<IReadOnlyList<DocumentoArquivoDTO>> ListarAsync(CategoriaDocumento categoria)
        {
            var service = CriarDriveService();
            var subpastaId = await ObterOuCriarSubpastaAsync(service, categoria);

            var request = service.Files.List();
            request.Q = $"'{subpastaId}' in parents and trashed = false";
            request.Fields = "files(id, name, size, createdTime, mimeType)";
            request.OrderBy = "createdTime desc";
            request.PageSize = 1000;
            AplicarSuporteSharedDrive(request);

            var response = await request.ExecuteAsync();

            return response.Files
                .Select(MapearArquivo)
                .ToList();
        }

        public async Task<DocumentoArquivoDTO> UploadAsync(
            CategoriaDocumento categoria,
            string nomeArquivo,
            string contentType,
            Stream conteudo)
        {
            var extensao = Path.GetExtension(nomeArquivo);
            if (!ExtensoesPermitidas.Contains(extensao))
                throw new DomainException($"Tipo de arquivo não permitido ({extensao}).");

            var service = CriarDriveService();
            var subpastaId = await ObterOuCriarSubpastaAsync(service, categoria);

            var metadata = new DriveFile
            {
                Name = nomeArquivo,
                Parents = new List<string> { subpastaId }
            };

            var request = service.Files.Create(metadata, conteudo, contentType);
            request.Fields = "id, name, size, createdTime, mimeType";
            request.SupportsAllDrives = true;

            var progresso = await request.UploadAsync();
            if (progresso.Status != UploadStatus.Completed)
            {
                _logger.LogError(progresso.Exception, "Falha ao enviar {Nome} para o Drive", nomeArquivo);
                throw new DomainException("Não foi possível enviar o documento ao Drive.");
            }

            return MapearArquivo(request.ResponseBody);
        }

        public async Task<DocumentoDownload?> BaixarAsync(string fileId)
        {
            var service = CriarDriveService();

            var metaRequest = service.Files.Get(fileId);
            metaRequest.Fields = "id, name, mimeType";
            metaRequest.SupportsAllDrives = true;

            DriveFile metadata;
            try
            {
                metadata = await metaRequest.ExecuteAsync();
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            var stream = new MemoryStream();
            var mediaRequest = service.Files.Get(fileId);
            mediaRequest.SupportsAllDrives = true;
            await mediaRequest.DownloadAsync(stream);
            stream.Position = 0;

            var contentType = string.IsNullOrEmpty(metadata.MimeType)
                ? "application/octet-stream"
                : metadata.MimeType;

            return new DocumentoDownload(stream, contentType, metadata.Name);
        }

        public async Task<bool> ExcluirAsync(string fileId)
        {
            var service = CriarDriveService();
            try
            {
                var request = service.Files.Delete(fileId);
                request.SupportsAllDrives = true;
                await request.ExecuteAsync();
                return true;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        // Acha (ou cria) a subpasta da categoria dentro da pasta raiz, para o
        // usuário só precisar compartilhar UMA pasta com a conta de serviço.
        private async Task<string> ObterOuCriarSubpastaAsync(DriveService service, CategoriaDocumento categoria)
        {
            if (_subpastas.TryGetValue(categoria, out var cacheId))
                return cacheId;

            var nome = categoria.GetDescription();
            var nomeEscapado = nome.Replace("'", "\\'");

            var request = service.Files.List();
            request.Q = $"name = '{nomeEscapado}' and mimeType = '{MimeTypePasta}' " +
                        $"and '{_config.PastaRaizId}' in parents and trashed = false";
            request.Fields = "files(id)";
            request.PageSize = 1;
            AplicarSuporteSharedDrive(request);

            var response = await request.ExecuteAsync();
            var existente = response.Files.FirstOrDefault();
            if (existente != null)
            {
                _subpastas[categoria] = existente.Id;
                return existente.Id;
            }

            var metadata = new DriveFile
            {
                Name = nome,
                MimeType = MimeTypePasta,
                Parents = new List<string> { _config.PastaRaizId }
            };
            var createRequest = service.Files.Create(metadata);
            createRequest.Fields = "id";
            createRequest.SupportsAllDrives = true;

            var pasta = await createRequest.ExecuteAsync();
            _subpastas[categoria] = pasta.Id;
            return pasta.Id;
        }

        private DriveService CriarDriveService()
        {
            if (_driveService != null)
                return _driveService;

            if (string.IsNullOrWhiteSpace(_config.PastaRaizId))
                throw new DomainException(
                    "Armazenamento no Google Drive não configurado (GoogleDrive:PastaRaizId ausente).");

            var credenciaisPath = Path.Combine(AppContext.BaseDirectory, _config.CredenciaisJson);
            if (!File.Exists(credenciaisPath))
                throw new DomainException(
                    "Credencial do Google Drive não encontrada no servidor.");

            GoogleCredential credential;
            using (var fileStream = new FileStream(credenciaisPath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, new UTF8Encoding(false)))
            {
                var json = reader.ReadToEnd();
                var bytes = Encoding.UTF8.GetBytes(json.TrimStart('﻿'));
                using var stream = new MemoryStream(bytes);
                credential = GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(DriveService.Scope.Drive);
            }

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "InstitutoTriboDeDavi"
            });

            return _driveService;
        }

        // Necessário para operar em Shared Drives (contas de serviço não têm
        // cota em My Drive, então os arquivos vivem num drive compartilhado).
        private static void AplicarSuporteSharedDrive(FilesResource.ListRequest request)
        {
            request.SupportsAllDrives = true;
            request.IncludeItemsFromAllDrives = true;
        }

        private static DocumentoArquivoDTO MapearArquivo(DriveFile file) => new()
        {
            Id = file.Id,
            Nome = file.Name,
            TamanhoBytes = file.Size ?? 0,
            DataCriacao = DateTime.TryParse(file.CreatedTimeRaw, out var data) ? data : null,
            MimeType = file.MimeType ?? string.Empty
        };
    }
}
