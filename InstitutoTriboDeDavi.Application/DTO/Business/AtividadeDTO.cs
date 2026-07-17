using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.DTO.Business
{
    public class AtividadeDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public TipoBloco Tipo { get; set; }
        public string? Descricao { get; set; }
        public string? Tags { get; set; }
        public string? Principio { get; set; }
        public string? ReferenciaBiblica { get; set; }
        public string? VideoUrl { get; set; }
    }
}
