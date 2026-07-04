using InstitutoTriboDeDavi.Domain.Common;
using System.ComponentModel;

namespace InstitutoTriboDeDavi.Domain.Enums
{
    public enum Parentesco
    {
        [Description("Pai")]
        Pai,
        [Description("Mãe")]
        Mae,
        [Description("Tio")]
        Tio,
        [Description("Tia")]
        Tia,
        [Description("Avo")]
        Avo,
        [Description("Outros")]
        Outros
    }
}
