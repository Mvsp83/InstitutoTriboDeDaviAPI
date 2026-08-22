namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        // Segundo fator (TOTP), quando o usuário tem 2FA ativo. Opcional na
        // primeira etapa: se vazio e o 2FA exigir, a API responde Requer2fa.
        public string? Codigo2fa { get; set; }
    }
}
