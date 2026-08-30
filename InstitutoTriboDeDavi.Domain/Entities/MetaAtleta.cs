using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Objetivo do atleta (peso de competição, faixa-alvo, resultado...).
    public class MetaAtleta : Base
    {
        public long AtletaId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime? Prazo { get; set; }
        public int Status { get; set; } // StatusMeta
        public DateTime? DataConclusao { get; set; }

        public override bool Validate() => true;
    }
}
