using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Uma avaliação física numa data: reúne vários indicadores medidos.
    public class AvaliacaoFisica : Base
    {
        public long AtletaId { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public List<IndicadorAvaliacao> Indicadores { get; set; } = new();

        public override bool Validate() => true;
    }

    // Um indicador medido (personalizável): nome livre, valor e unidade.
    // Ex.: { "Flexão de braço", 30, "reps" }, { "Peso", 62.5, "kg" }.
    public class IndicadorAvaliacao : Base
    {
        public long AvaliacaoFisicaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Unidade { get; set; } = string.Empty;

        public override bool Validate() => true;
    }
}
