namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Autenticação em dois fatores por TOTP (RFC 6238) — o mesmo padrão do
    // Google Authenticator/Authy. Sem dependência de SMTP/SMS.
    public interface ITotpService
    {
        // Novo secret aleatório em base32, para guardar no usuário.
        string GerarSecret();

        // URI otpauth:// que o app autenticador lê via QR Code.
        string GerarUri(string secret, string conta, string emissor);

        // Valida o código de 6 dígitos contra o secret, com tolerância de uma
        // janela de tempo para o relógio do celular.
        bool Validar(string secret, string codigo);
    }
}
