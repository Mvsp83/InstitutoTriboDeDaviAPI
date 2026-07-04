namespace InstitutoTriboDeDavi.Infrastructure.Configuration
{
    public class GoogleSheetsConfig
    {
        public string CredenciaisJson { get; set; }
        public string HorarioExecucao { get; set; } = "02:00";
        public List<PlanilhaConfig> Planilhas { get; set; } = new();
    }

    public class PlanilhaConfig
    {
        public long PoloId { get; set; }
        public string PoloNome { get; set; }
        public string SpreadsheetId { get; set; }
        public string NomeAba { get; set; }
    }
}
