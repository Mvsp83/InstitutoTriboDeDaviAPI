using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Domain.Enum;

namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    public class AlunoViewModel
    {
        public string Nome { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public double Peso { get; set; }
        public Faixa Faixa { get; set; }
        public long EnderecoId { get; set; }
        public long ResponsavelId { get; set; }
        public string Celular { get; set; }
    }
}
