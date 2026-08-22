using InstitutoTriboDeDavi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class UsuarioDTO
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "O Login não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Login deve ter no mínimo 3 caracteres.")]
        [MaxLength(20, ErrorMessage = "O Login deve ter no máximo 20 caracteres.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "O Password não pode ser vazio.")]
        [MinLength(8, ErrorMessage = "O Password deve ter no mínimo 8 caracteres.")]
        [MaxLength(100, ErrorMessage = "O Password deve ter no máximo 100 caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O Email não pode ser vazio.")]
        [EmailAddress]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
        ErrorMessage = "O Email informado não é válido.")]
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public long? PoloId { get; set; }
        public string PoloNome { get; set; }
        public string? Avatar { get; set; }
        // 2FA ativo e confirmado — usado no login para decidir se exige o código.
        public bool TotpConfirmado { get; set; }
    }
}
