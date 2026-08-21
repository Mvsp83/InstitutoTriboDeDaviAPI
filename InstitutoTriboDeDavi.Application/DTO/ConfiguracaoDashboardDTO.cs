namespace InstitutoTriboDeDavi.Application.DTO
{
    public class ConfiguracaoDashboardDTO
    {
        public long Id { get; set; }
        // Preenchido pelo servidor com o usuário autenticado; ignorado no body.
        public string? UsuarioLogin { get; set; }
        // JSON do layout (ordem + widgets ocultos). O portal define o formato.
        public string Layout { get; set; }
    }
}
