using InstitutoTriboDeDavi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class AlunoDTO
    {
        //[Required(ErrorMessage = "O Id não pode ser vazio")]
        //[Range(1, long.MaxValue, ErrorMessage = "O Id não pode ser menor que 1")]
        public long Id { get; set; }
        // Inscrito como adulto (ficha de adultos).
        public bool EhAdulto { get; set; }

        [Required(ErrorMessage = "O Nome não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Nome deve ter no mínimo 3 caracteres.")]
        [MaxLength(120, ErrorMessage = "O Nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public double Peso { get; set; }
        public double? Altura { get; set; }
        public Faixa Faixa { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Celular { get; set; }
        public string Telefone2 { get; set; }
        public string Responsavel { get; set; }
        public Parentesco Parentesco { get; set; }
        public string RGResponsavel { get; set; }
        public string CPFResponsavel { get; set; }
        public string Escola { get; set; }
        public string Serie { get; set; }
        public string Periodo { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        // Autorização de uso de imagem/voz (LGPD): null = não informado,
        // true = autoriza, false = não autoriza.
        public bool? AutorizaImagem { get; set; }
    }
}
