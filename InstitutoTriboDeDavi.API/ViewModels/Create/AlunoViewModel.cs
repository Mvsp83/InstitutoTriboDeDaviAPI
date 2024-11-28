using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Domain.Enums;

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
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Celular { get; set; }
        public string Responsavel { get; set; }
        public Parentesco Parentesco { get; set; }
        public string RGResponsavel { get; set; }
        public string CPFResponsavel { get; set; }
        public string Escola { get; set; }
        public string Periodo { get; set; }
        public long PoloId { get; set; }
    }
}
