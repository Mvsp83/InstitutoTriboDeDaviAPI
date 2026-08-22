namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    // Corpo dos endpoints de refresh/logout: o refresh token em claro que o
    // cliente guardou no login.
    public class RefreshTokenViewModel
    {
        public string? RefreshToken { get; set; }
    }
}
