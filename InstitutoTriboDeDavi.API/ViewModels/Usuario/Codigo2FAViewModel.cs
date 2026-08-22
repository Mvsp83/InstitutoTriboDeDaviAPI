namespace InstitutoTriboDeDavi.API.ViewModels.Usuario
{
    // Código de 6 dígitos do app autenticador, para confirmar/desativar o 2FA.
    public class Codigo2FAViewModel
    {
        public string Codigo { get; set; } = string.Empty;
    }
}
