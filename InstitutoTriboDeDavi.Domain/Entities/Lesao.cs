using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Registro de lesão/saúde do atleta: o que houve, gravidade, e a
    // recuperação (retorno previsto/efetivo).
    public class Lesao : Base
    {
        public long AtletaId { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty; // região do corpo
        public int Gravidade { get; set; } // GravidadeLesao
        public DateTime? DataRetorno { get; set; }
        public bool Recuperado { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public override bool Validate() => true;
    }
}
