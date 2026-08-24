using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace InstitutoTriboDeDavi.Infrastructure.Video
{
    // Traz a legenda de um vídeo do YouTube traduzida para PT usando a tradução
    // automática do próprio YouTube (parâmetro tlang=pt no endpoint de legendas).
    // É best-effort: usa endpoints não-oficiais; qualquer falha vira um aviso
    // amigável em vez de erro. Só funciona para vídeos que TÊM legenda.
    public class VideoTranscricaoService : IVideoTranscricaoService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<VideoTranscricaoService> _logger;

        public VideoTranscricaoService(IHttpClientFactory httpFactory, ILogger<VideoTranscricaoService> logger)
        {
            _httpFactory = httpFactory;
            _logger = logger;
        }

        public async Task<TranscricaoResultado> ObterTraduzidaAsync(string videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId) || !Regex.IsMatch(videoId, "^[A-Za-z0-9_-]{11}$"))
                return new TranscricaoResultado("", "Vídeo inválido.");

            try
            {
                var http = _httpFactory.CreateClient();
                http.Timeout = TimeSpan.FromSeconds(20);
                // UA de navegador + cookie de consentimento evitam a página de aviso.
                http.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
                http.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                http.DefaultRequestHeaders.Add("Cookie", "CONSENT=YES+cb");

                var html = await http.GetStringAsync($"https://www.youtube.com/watch?v={videoId}&hl=en");

                var baseUrl = ExtrairPrimeiraLegenda(html);
                if (baseUrl == null)
                    return new TranscricaoResultado("", "Este vídeo não tem legenda disponível para trazer a transcrição.");

                // tlang=pt pede a tradução automática do YouTube para português.
                var urlLegenda = baseUrl + "&tlang=pt";
                var xml = await http.GetStringAsync(urlLegenda);

                var texto = TextoDoXml(xml);
                if (string.IsNullOrWhiteSpace(texto))
                    return new TranscricaoResultado("", "Não foi possível ler a legenda traduzida deste vídeo.");

                return new TranscricaoResultado(texto, "");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao obter transcrição do vídeo {VideoId}.", videoId);
                return new TranscricaoResultado("", "Não foi possível trazer a transcrição agora. Tente novamente ou copie manualmente.");
            }
        }

        // Extrai o baseUrl da primeira faixa de legenda do JSON embutido na página
        // (chave "captionTracks"). Faz varredura de colchetes para pegar o array
        // completo (regex simples quebraria com URLs longas).
        private static string ExtrairPrimeiraLegenda(string html)
        {
            const string marca = "\"captionTracks\":";
            var i = html.IndexOf(marca, StringComparison.Ordinal);
            if (i < 0) return null;

            var inicio = html.IndexOf('[', i);
            if (inicio < 0) return null;

            var profundidade = 0;
            var fim = -1;
            for (var j = inicio; j < html.Length; j++)
            {
                if (html[j] == '[') profundidade++;
                else if (html[j] == ']')
                {
                    profundidade--;
                    if (profundidade == 0) { fim = j; break; }
                }
            }
            if (fim < 0) return null;

            var arrayJson = html.Substring(inicio, fim - inicio + 1);
            try
            {
                using var doc = JsonDocument.Parse(arrayJson);
                var primeiro = doc.RootElement.EnumerateArray().FirstOrDefault();
                if (primeiro.ValueKind != JsonValueKind.Object) return null;
                if (!primeiro.TryGetProperty("baseUrl", out var url)) return null;
                return url.GetString();
            }
            catch
            {
                return null;
            }
        }

        // Junta o texto dos elementos <text> da legenda (XML timedtext), fazendo
        // o HTML-decode (a legenda vem com entidades como &#39;).
        private static string TextoDoXml(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                var linhas = doc.Descendants("text")
                    .Select(t => WebUtility.HtmlDecode(t.Value).Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s));
                return string.Join(" ", linhas).Trim();
            }
            catch
            {
                return "";
            }
        }
    }
}
