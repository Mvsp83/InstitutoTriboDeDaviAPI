using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.System.DTO
{
    public class PaisDTO
    {
        [Required(ErrorMessage = "O Id não pode ser vazio")]
        [Range(1, long.MaxValue, ErrorMessage = "O Id não pode ser menor que 1")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O Nome não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Nome deve ter no mínimo 3 caracteres.")]
        [MaxLength(120, ErrorMessage = "O Nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A Sigla não pode ser vazia.")]
        [MinLength(2, ErrorMessage = "A Sigla deve ter no mínimo 2 caracteres.")]
        [MaxLength(2, ErrorMessage = "A Sigla deve ter no máximo 2 caracteres.")]
        public string Sigla { get; set; }
    }
}
