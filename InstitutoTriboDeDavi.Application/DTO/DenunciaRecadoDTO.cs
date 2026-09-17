namespace InstitutoTriboDeDavi.Application.DTO
{
    // Item da fila de denúncias (visão da equipe). RecadoTitulo é resolvido no
    // serviço para a equipe reconhecer o anúncio sem outra consulta.
    public class DenunciaRecadoDTO
    {
        public long Id { get; set; }
        public long RecadoId { get; set; }
        public string RecadoTitulo { get; set; }
        public string Motivo { get; set; }
        public string DenunciadoPor { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
