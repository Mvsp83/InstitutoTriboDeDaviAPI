namespace InstitutoTriboDeDavi.Infrastructure.Configuration
{
    // Espelha o padrão de GoogleSheetsConfig. A credencial é a mesma conta de
    // serviço dos Sheets (só precisa da Drive API habilitada e da pasta
    // compartilhada com o client_email da conta de serviço).
    public class GoogleDriveConfig
    {
        public string CredenciaisJson { get; set; } = string.Empty;

        // ID de UMA pasta (em um Shared Drive) compartilhada com a conta de
        // serviço. As subpastas por categoria são criadas dentro dela.
        public string PastaRaizId { get; set; } = string.Empty;
    }
}
