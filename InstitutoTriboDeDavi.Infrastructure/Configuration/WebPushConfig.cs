namespace InstitutoTriboDeDavi.Infrastructure.Configuration
{
    // Web Push (VAPID). As chaves NÃO ficam no appsettings — dev: user-secrets;
    // produção: env WebPush__PublicKey / WebPush__PrivateKey. Gere o par com
    // qualquer gerador VAPID (ex.: `npx web-push generate-vapid-keys`).
    public class WebPushConfig
    {
        // Chave pública VAPID (base64url) — vai também para o front (applicationServerKey).
        public string PublicKey { get; set; } = string.Empty;
        // Chave privada VAPID (base64url) — segredo, só no servidor.
        public string PrivateKey { get; set; } = string.Empty;
        // "subject" do VAPID: mailto: ou URL do responsável pelo envio.
        public string Subject { get; set; } = "mailto:contato@tribodedavi.org";
    }
}
