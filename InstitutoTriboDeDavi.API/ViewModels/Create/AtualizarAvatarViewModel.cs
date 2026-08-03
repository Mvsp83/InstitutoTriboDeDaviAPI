namespace InstitutoTriboDeDavi.API.ViewModels.Usuario
{
    // Corpo do PUT meu-avatar: preset ("preset:7"), data URI de imagem ou null
    // (para remover). A validação de formato/tamanho fica no UsuarioService.
    public class AtualizarAvatarViewModel
    {
        public string? Avatar { get; set; }
    }
}
