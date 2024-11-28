using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.API.ViewModels.Business
{
    public class PresencaViewModel
    {
        public long AlunoId { get; set; }
        public long PoloId { get; set; }
        public DateTime Data { get; set; }
        public bool EstaPresente { get; set; }
        public string Observacoes { get; set; }
        public long AulaId { get; set; }
    }
}
