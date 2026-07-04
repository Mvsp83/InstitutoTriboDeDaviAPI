using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.API.ViewModels.Usuario
{
    public class UsuarioViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public long? PoloId { get; set; }
        public string PoloNome { get; set; }
    }
}
