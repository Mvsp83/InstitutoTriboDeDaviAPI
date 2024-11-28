using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using System.ComponentModel;

namespace InstitutoTriboDeDavi.System.Domain.Enums
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
