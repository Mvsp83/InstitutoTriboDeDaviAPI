namespace InstitutoTriboDeDavi.Infrastructure.Configuration
{
    // Integração com o Google Drive de uma conta Gmail comum (sem Workspace).
    // Como conta de serviço não tem cota de armazenamento, o servidor age como
    // o próprio usuário via OAuth 2.0 (refresh token) com escopo drive.file — o
    // app só enxerga/gerencia os arquivos que ele mesmo cria.
    public class GoogleDriveConfig
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        // Obtido uma única vez no consentimento OAuth; não expira enquanto a tela
        // de consentimento estiver "Em produção" e o acesso não for revogado.
        public string RefreshToken { get; set; } = string.Empty;

        // Pasta raiz criada no Drive do usuário; as subpastas por categoria são
        // criadas dentro dela.
        public string PastaRaiz { get; set; } = "Instituto Tribo de Davi - Documentos";
    }
}
