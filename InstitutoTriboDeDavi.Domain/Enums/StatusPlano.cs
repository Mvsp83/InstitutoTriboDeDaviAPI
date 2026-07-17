using System.ComponentModel;

namespace InstitutoTriboDeDavi.Domain.Enums
{
    public enum StatusPlano
    {
        [Description("Rascunho")]
        Rascunho,
        [Description("Pronto")]
        Pronto,
        [Description("Aplicado")]
        Aplicado
    }
}
