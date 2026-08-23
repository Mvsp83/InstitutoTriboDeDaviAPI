using System.Net;
using System.Text.Json;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPush;
using DomainPushSubscription = InstitutoTriboDeDavi.Domain.Entities.Business.PushSubscription;
using LibPushSubscription = WebPush.PushSubscription;

namespace InstitutoTriboDeDavi.Infrastructure.Push
{
    // Envio de Web Push (protocolo VAPID) via biblioteca WebPush. Sem chaves
    // configuradas, o serviço fica inativo (EstaConfigurado = false) e o envio
    // é ignorado — no mesmo modelo do email/Pix, que só funcionam quando o
    // usuário preenche a configuração.
    public class PushService : IPushService
    {
        private readonly IPushSubscriptionRepository _repository;
        private readonly WebPushConfig _config;
        private readonly ILogger<PushService> _logger;
        private readonly VapidDetails _vapid;
        private readonly WebPushClient _client = new();

        public PushService(
            IPushSubscriptionRepository repository,
            IOptions<WebPushConfig> config,
            ILogger<PushService> logger)
        {
            _repository = repository;
            _config = config.Value;
            _logger = logger;

            if (EstaConfigurado)
                _vapid = new VapidDetails(_config.Subject, _config.PublicKey, _config.PrivateKey);
        }

        public bool EstaConfigurado =>
            !string.IsNullOrWhiteSpace(_config.PublicKey) &&
            !string.IsNullOrWhiteSpace(_config.PrivateKey);

        public string ChavePublica => _config.PublicKey ?? string.Empty;

        public async Task InscreverAsync(string usuarioLogin, string endpoint, string p256dh, string auth)
        {
            // Um endpoint é único por dispositivo: se já existe, atualiza o dono e
            // as chaves em vez de duplicar (o navegador pode renovar as chaves).
            var existente = await _repository.ObterPorEndpointAsync(endpoint);
            if (existente != null)
            {
                existente.UsuarioLogin = usuarioLogin;
                existente.P256dh = p256dh;
                existente.Auth = auth;
                await _repository.UpdateAsync(existente);
                return;
            }

            await _repository.CreateAsync(new DomainPushSubscription
            {
                UsuarioLogin = usuarioLogin,
                Endpoint = endpoint,
                P256dh = p256dh,
                Auth = auth,
                DataCriacao = DateTime.Now,
            });
        }

        public Task DesinscreverAsync(string endpoint) =>
            _repository.RemoverPorEndpointAsync(endpoint);

        public async Task<int> EnviarParaUsuarioAsync(string usuarioLogin, string titulo, string corpo, string url)
        {
            if (!EstaConfigurado)
            {
                _logger.LogInformation("Web Push não configurado — envio ignorado.");
                return 0;
            }

            var inscricoes = await _repository.ObterPorUsuarioAsync(usuarioLogin);
            if (inscricoes.Count == 0)
                return 0;

            var payload = JsonSerializer.Serialize(new { title = titulo, body = corpo, url });
            var enviados = 0;

            foreach (var inscricao in inscricoes)
            {
                try
                {
                    var sub = new LibPushSubscription(inscricao.Endpoint, inscricao.P256dh, inscricao.Auth);
                    await _client.SendNotificationAsync(sub, payload, _vapid);
                    enviados++;
                }
                catch (WebPushException ex) when (
                    ex.StatusCode == HttpStatusCode.NotFound ||
                    ex.StatusCode == HttpStatusCode.Gone)
                {
                    // 404/410 = inscrição expirada/cancelada no navegador: limpa.
                    await _repository.RemoverPorEndpointAsync(inscricao.Endpoint);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao enviar push para {Login}.", usuarioLogin);
                }
            }

            return enviados;
        }
    }
}
