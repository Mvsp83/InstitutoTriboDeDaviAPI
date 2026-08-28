using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Configuração, por polo, do fluxo de publicação das fotos de treino.
    // RequerAutorizacao = true (padrão): a foto entra pendente e o admin aprova.
    // false: a foto do polo já entra publicada (o polo é confiável).
    public class PoloFotoConfig : Base
    {
        public long PoloId { get; set; }
        public bool RequerAutorizacao { get; set; } = true;

        public override bool Validate() => true;
    }
}
