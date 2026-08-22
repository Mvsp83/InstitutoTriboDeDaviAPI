namespace InstitutoTriboDeDavi.Application.DTO
{
    // Dados para o usuário cadastrar o 2FA no app autenticador: o secret (para
    // digitar à mão, se preferir) e a URI otpauth:// (que vira o QR Code).
    public class Setup2FADTO
    {
        public string Secret { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
    }
}
