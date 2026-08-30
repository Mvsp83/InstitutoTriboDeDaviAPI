namespace InstitutoTriboDeDavi.Application.DTO
{
    public class EventoCalendarioDTO
    {
        public long Id { get; set; }
        public int Ano { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataFim { get; set; }
        public string Titulo { get; set; }
        public int Tipo { get; set; }
        public string Descricao { get; set; }
        public long? PoloId { get; set; }
        public bool Interno { get; set; }
        public bool Notificar { get; set; }
        public string EmailsNotificacao { get; set; }
        public int DiasAntecedencia { get; set; }
    }
}
