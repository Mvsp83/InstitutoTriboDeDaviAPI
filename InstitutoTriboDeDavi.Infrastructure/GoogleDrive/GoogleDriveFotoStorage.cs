using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace InstitutoTriboDeDavi.Infrastructure.GoogleDrive
{
    // Storage das fotos de treino no Google Drive, numa subpasta própria
    // ("Fotos de Treino") sob a pasta raiz do app. Aceita apenas imagens.
    public class GoogleDriveFotoStorage : IFotoStorage
    {
        private const string MimeTypePasta = "application/vnd.google-apps.folder";
        private const string NomeSubpasta = "Fotos de Treino";

        private static readonly HashSet<string> ExtensoesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private readonly GoogleDriveConfig _config;
        private readonly ILogger<GoogleDriveFotoStorage> _logger;

        private DriveService _driveService;
        private string _subpastaId;

        public GoogleDriveFotoStorage(
            IOptions<GoogleDriveConfig> config,
            ILogger<GoogleDriveFotoStorage> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task<string> UploadAsync(string nomeArquivo, string contentType, Stream conteudo)
        {
            var extensao = Path.GetExtension(nomeArquivo);
            if (!ExtensoesPermitidas.Contains(extensao))
                throw new DomainException($"Tipo de imagem não permitido ({extensao}). Use JPG, PNG ou WebP.");

            var service = CriarDriveService();
            var subpastaId = await ObterOuCriarSubpastaAsync(service);

            var metadata = new DriveFile
            {
                Name = nomeArquivo,
                Parents = new List<string> { subpastaId }
            };

            var request = service.Files.Create(metadata, conteudo, contentType);
            request.Fields = "id";

            var progresso = await request.UploadAsync();
            if (progresso.Status != UploadStatus.Completed)
            {
                _logger.LogError(progresso.Exception, "Falha ao enviar a foto {Nome} para o Drive", nomeArquivo);
                throw new DomainException("Não foi possível enviar a foto ao Drive.");
            }

            return request.ResponseBody.Id;
        }

        public async Task<FotoDownload> BaixarAsync(string fileId)
        {
            var service = CriarDriveService();

            var metaRequest = service.Files.Get(fileId);
            metaRequest.Fields = "id, mimeType";

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
            await service.Files.Get(fileId).DownloadAsync(stream);
            stream.Position = 0;

            var contentType = string.IsNullOrEmpty(metadata.MimeType)
                ? "image/jpeg"
                : metadata.MimeType;

            return new FotoDownload(stream, contentType);
        }

        public async Task ExcluirAsync(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return;
            var service = CriarDriveService();
            try
            {
                await service.Files.Delete(fileId).ExecuteAsync();
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Arquivo já não existe: nada a fazer.
            }
        }

        private async Task<string> ObterOuCriarSubpastaAsync(DriveService service)
        {
            if (_subpastaId != null) return _subpastaId;

            var raizId = await ObterOuCriarPastaAsync(service, _config.PastaRaiz, parenteId: null);
            _subpastaId = await ObterOuCriarPastaAsync(service, NomeSubpasta, parenteId: raizId);
            return _subpastaId;
        }

        private async Task<string> ObterOuCriarPastaAsync(DriveService service, string nome, string parenteId)
        {
            var nomeEscapado = nome.Replace("'", "\\'");
            var filtroParente = parenteId != null ? $" and '{parenteId}' in parents" : "";

            var request = service.Files.List();
            request.Q = $"name = '{nomeEscapado}' and mimeType = '{MimeTypePasta}' and trashed = false{filtroParente}";
            request.Fields = "files(id)";
            request.PageSize = 1;

            var response = await request.ExecuteAsync();
            var existente = response.Files.FirstOrDefault();
            if (existente != null) return existente.Id;

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

        private DriveService CriarDriveService()
        {
            if (_driveService != null) return _driveService;

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
    }
}
