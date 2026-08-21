using System.ComponentModel;

namespace InstitutoTriboDeDavi.Domain.Enums
{
    // Quem vê o aviso. O valor int trafega no DTO.
    public enum PublicoAviso
    {
        [Description("Todos")] Todos,
        [Description("Professores")] Professores,
        [Description("Supervisores")] Supervisores
    }
}
