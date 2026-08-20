using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
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
        private string? _pastaRaizId;
        // Cache dos IDs de subpasta por categoria dentro do escopo.
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
                await service.Files.Delete(fileId).ExecuteAsync();
                return true;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        // Acha (ou cria) a subpasta da categoria dentro da pasta raiz do app.
        private async Task<string> ObterOuCriarSubpastaAsync(DriveService service, CategoriaDocumento categoria)
        {
            if (_subpastas.TryGetValue(categoria, out var cacheId))
                return cacheId;

            var raizId = await ObterOuCriarPastaAsync(service, _config.PastaRaiz, parenteId: null);
            var subpastaId = await ObterOuCriarPastaAsync(service, categoria.GetDescription(), parenteId: raizId);

            _subpastas[categoria] = subpastaId;
            return subpastaId;
        }

        // Com escopo drive.file, a busca só enxerga itens que o próprio app criou
        // — suficiente porque tanto a raiz quanto as subpastas são criadas aqui.
        private async Task<string> ObterOuCriarPastaAsync(DriveService service, string nome, string? parenteId)
        {
            var nomeEscapado = nome.Replace("'", "\\'");
            var filtroParente = parenteId != null ? $" and '{parenteId}' in parents" : "";

            var request = service.Files.List();
            request.Q = $"name = '{nomeEscapado}' and mimeType = '{MimeTypePasta}' and trashed = false{filtroParente}";
            request.Fields = "files(id)";
            request.PageSize = 1;

            var response = await request.ExecuteAsync();
            var existente = response.Files.FirstOrDefault();
            if (existente != null)
                return existente.Id;

            var metadata = new DriveFile
            {
                Name = nome,
                MimeType = MimeTypePasta,
                Parents = parenteId != null ? new List<string> { parenteId } : null
            };
            var createRequest = service.Files.Create(metadata);
            createRequest.Fields = "id";

            var pasta = await createRequest.ExecuteAsync();
            return pasta.Id;
        }

        // Credencial de USUÁRIO (OAuth) a partir do refresh token — funciona com
        // Gmail comum, sem Workspace/Shared Drive. O access token é renovado
        // automaticamente pela biblioteca.
        private DriveService CriarDriveService()
        {
            if (_driveService != null)
                return _driveService;

            if (string.IsNullOrWhiteSpace(_config.ClientId) ||
                string.IsNullOrWhiteSpace(_config.ClientSecret) ||
                string.IsNullOrWhiteSpace(_config.RefreshToken))
            {
                throw new DomainException(
                    "Integração com o Google Drive não configurada " +
                    "(GoogleDrive:ClientId/ClientSecret/RefreshToken ausentes).");
            }

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _config.ClientId,
                    ClientSecret = _config.ClientSecret
                },
                Scopes = new[] { DriveService.Scope.DriveFile }
            });

            var token = new TokenResponse { RefreshToken = _config.RefreshToken };
            var credential = new UserCredential(flow, "instituto", token);

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "InstitutoTriboDeDavi"
            });

            return _driveService;
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
