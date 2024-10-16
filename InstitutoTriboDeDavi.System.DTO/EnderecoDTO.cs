using InstitutoTriboDeDavi.System.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.System.DTO
{
    public class EnderecoDTO
    {
        [Required(ErrorMessage = "O Id não pode ser vazio")]
        [Range(1, long.MaxValue, ErrorMessage = "O Id não pode ser menor que 1")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O Logradouro não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Logradouro deve ter no mínimo 3 caracteres.")]
        [MaxLength(120, ErrorMessage = "O Logradouro deve ter no máximo 120 caracteres.")]
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public long BairroId { get; set; }
        public Bairro Bairro { get; set; }
        public long CidadeId { get; set; }
        public Cidade Cidade { get; set; }
        public long EstadoId { get; set; }
        public Estado Estado { get; set; }
        public long PaisId { get; set; }
        public Pais Pais { get; set; }
    }
}
