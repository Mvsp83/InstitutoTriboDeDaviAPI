using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.DTO.Queries
{
    public class HistoricoAtividadeDTO
    {
        public long AtividadeId { get; set; }
        public string Nome { get; set; }
        public TipoBloco Tipo { get; set; }
        public DateTime UltimaData { get; set; }
        public int Vezes { get; set; }
    }
}
