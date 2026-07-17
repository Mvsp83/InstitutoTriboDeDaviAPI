using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.DTO.Business
{
    public class BlocoDoModeloDTO
    {
        public long Id { get; set; }
        public long ModeloDeAulaId { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public TipoBloco Tipo { get; set; }
        public int DuracaoMinutos { get; set; }
        public string? Descricao { get; set; }
    }
}
