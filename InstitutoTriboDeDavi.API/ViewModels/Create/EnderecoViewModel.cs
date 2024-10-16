using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    public class EnderecoViewModel
    {
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public long BairroId { get; set; }
        public long CidadeId { get; set; }
        public long EstadoId { get; set; }
        public long PaisId { get; set; }
    }
}
