using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Entrada do diário técnico do atleta (registro do professor no tempo).
    public class AnotacaoAtleta : Base
    {
        public long AtletaId { get; set; }
        public DateTime Data { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty; // login de quem anotou

        public override bool Validate() => true;
    }
}
