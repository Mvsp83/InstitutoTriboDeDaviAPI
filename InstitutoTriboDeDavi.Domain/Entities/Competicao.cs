using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Participação do atleta numa competição e o resultado.
    public class Competicao : Base
    {
        public long AtletaId { get; set; }
        public DateTime Data { get; set; }
        public string Evento { get; set; } = string.Empty;
        public string CategoriaPeso { get; set; } = string.Empty;
        // 0 = sem pódio/não classificado; 1, 2, 3... = colocação.
        public int Colocacao { get; set; }
        public int Lutas { get; set; }
        public int Vitorias { get; set; }
        public int Finalizacoes { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public override bool Validate() => true;
    }
}
