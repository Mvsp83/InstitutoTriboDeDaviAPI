namespace InstitutoTriboDeDavi.Application.DTO.Business
{
    public class AulaDTO
    {
        public long Id { get; set; }
        public long PoloId { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public bool PresencaSalva { get; set; }
        public int Turma { get; set; }
    }
}

