using System.ComponentModel;

namespace InstitutoTriboDeDavi.Domain.Enums
{
    // Tipos de evento do calendário oficial. O valor int é o que trafega no DTO;
    // o portal React tem seus próprios rótulos/cores por tipo.
    public enum TipoEventoCalendario
    {
        [Description("Início das aulas")]
        InicioAulas,
        [Description("Término das aulas")]
        TerminoAulas,
        [Description("Graduação")]
        Graduacao,
        [Description("Data comemorativa")]
        Comemorativa,
        [Description("DRE")]
        Dre,
        [Description("Balanço")]
        Balanco,
        [Description("Fechamento financeiro")]
        FechamentoFinanceiro,
        [Description("Assembleia")]
        Assembleia,
        [Description("Outro")]
        Outro
    }
}
