using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.System.DTO
{
    public class AlunoDTO
    {
        [Required(ErrorMessage = "O Id não pode ser vazio")]
        [Range(1, long.MaxValue, ErrorMessage = "O Id não pode ser menor que 1")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O Nome não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Nome deve ter no mínimo 3 caracteres.")]
        [MaxLength(120, ErrorMessage = "O Nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public double Peso { get; set; }
        public Faixa Faixa { get; set; }
        public long EnderecoId { get; set; }
        public Endereco Endereco { get; set; }
        public long ResponsavelId { get; set; }
        public Responsavel Responsavel { get; set; }
        public string Celular { get; set; }
    }
}
