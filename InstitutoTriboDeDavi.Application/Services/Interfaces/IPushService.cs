using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IPushService
    {
        // true quando as chaves VAPID estão configuradas (senão o envio é no-op).
        bool EstaConfigurado { get; }

        // Chave pública VAPID, entregue ao front para assinar a inscrição.
        string ChavePublica { get; }

        // Registra/atualiza a inscrição de um dispositivo do usuário.
        Task InscreverAsync(string usuarioLogin, string endpoint, string p256dh, string auth);

        // Remove a inscrição de um dispositivo (ex.: usuário desligou as notificações).
        Task DesinscreverAsync(string endpoint);

        // Envia uma notificação a todos os dispositivos do usuário. Retorna quantos
        // envios tiveram sucesso; inscrições expiradas são removidas no caminho.
        Task<int> EnviarParaUsuarioAsync(string usuarioLogin, string titulo, string corpo, string url);
    }
}
