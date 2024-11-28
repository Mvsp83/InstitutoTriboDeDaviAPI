using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    public class PoloViewModel
    {
        public string Nome { get; set; }
        public string Informacoes { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
    }
}
