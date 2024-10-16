using InstitutoTriboDeDavi.System.Domain.Enums;

namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    public class ResponsavelViewModel
    {
        public string Nome { get; set; }
        public Parentesco Parentesco { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
    }
}
