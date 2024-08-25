using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.API.ViewModels.Usuario
{
    public class UpdateUsuarioViewModel
    {
        [Required(ErrorMessage = "O Id não pode ser vazio")]
        [Range(1, long.MaxValue, ErrorMessage = "O Id não pode ser menor que 1")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O Login não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Login deve ter no mínimo 3 caracteres.")]
        [MaxLength(20, ErrorMessage = "O Login deve ter no máximo 20 caracteres.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "O Password não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Password deve ter no mínimo 3 caracteres.")]
        [MaxLength(20, ErrorMessage = "O Password deve ter no máximo 20 caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O Email não pode ser vazio.")]
        [EmailAddress]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
        ErrorMessage = "O Email informado não é válido.")]
        public string Email { get; set; }

        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
