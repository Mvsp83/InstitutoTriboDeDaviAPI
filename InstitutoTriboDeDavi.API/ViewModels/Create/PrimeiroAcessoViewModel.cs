namespace InstitutoTriboDeDavi.API.ViewModels.Usuario
{
    // Dados do primeiro admin (bootstrap). O Token só é exigido quando
    // "Setup:Token" (env Setup__Token) está configurado — em produção.
    public class PrimeiroAcessoViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
