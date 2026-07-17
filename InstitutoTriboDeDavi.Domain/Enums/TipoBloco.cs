using System.ComponentModel;

namespace InstitutoTriboDeDavi.Domain.Enums
{
    public enum TipoBloco
    {
        [Description("Aquecimento")]
        Aquecimento,
        [Description("Posições")]
        Posicoes,
        [Description("Lutas")]
        Lutas,
        [Description("Dinâmicas")]
        Dinamicas,
        [Description("Mensagem Final")]
        MensagemFinal,
        [Description("Outro")]
        Outro
    }
}
