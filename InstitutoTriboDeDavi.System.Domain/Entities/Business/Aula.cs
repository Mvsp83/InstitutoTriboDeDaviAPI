using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;

namespace InstitutoTriboDeDavi.System.Domain.Entities.Business
{
    public class Aula : Base
    {
        public long PoloId { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public bool PresencaSalva { get; set; }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
