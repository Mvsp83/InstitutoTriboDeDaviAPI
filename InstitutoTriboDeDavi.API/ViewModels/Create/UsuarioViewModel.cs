using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.API.ViewModels.Usuario
{
    public class UsuarioViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
