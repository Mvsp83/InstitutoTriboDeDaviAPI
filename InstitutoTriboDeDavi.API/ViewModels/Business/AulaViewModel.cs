using InstitutoTriboDeDavi.System.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.API.ViewModels.Business
{
    public class AulaViewModel
    {
        public long PoloId { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public bool PresencaSalva { get; set; }
    }
}
