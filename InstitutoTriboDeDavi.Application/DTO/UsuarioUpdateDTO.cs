using InstitutoTriboDeDavi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // DTO exclusivo do update: campos opcionais nulos significam "manter o valor atual",
    // evitando que uma edição parcial sobrescreva Role, Polo ou a senha do usuário.
    public class UsuarioUpdateDTO
    {
        [Required(ErrorMessage = "O Id não pode ser vazio.")]
        [Range(1, long.MaxValue, ErrorMessage = "O Id não pode ser menor que 1.")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O Login não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Login deve ter no mínimo 3 caracteres.")]
        [MaxLength(20, ErrorMessage = "O Login deve ter no máximo 20 caracteres.")]
        public string Login { get; set; }

        // Opcional: quando não enviado (ou vazio), a senha atual é mantida
        [MinLength(3, ErrorMessage = "O Password deve ter no mínimo 3 caracteres.")]
        [MaxLength(20, ErrorMessage = "O Password deve ter no máximo 20 caracteres.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "O Email não pode ser vazio.")]
        [EmailAddress(ErrorMessage = "O Email informado não é válido.")]
        public string Email { get; set; }

        // Opcionais: quando não enviados, os valores atuais são mantidos
        public UserRole? Role { get; set; }
        public long? PoloId { get; set; }
        public string? PoloNome { get; set; }
        // Permissão de acesso ao Programa de Graduação (professor).
        public bool? PermiteGraduacao { get; set; }
    }
}
