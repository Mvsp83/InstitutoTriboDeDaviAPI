using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Resultado de uma rotação bem-sucedida: de quem é a sessão e o novo token
    // em claro para devolver ao cliente.
    public record RotacaoRefresh(long UsuarioId, string NovoTokenRaw);

    public interface IRefreshTokenService
    {
        // Emite um novo refresh token para o usuário e devolve o valor em claro
        // (só o hash é guardado). Chamado no login.
        Task<string> EmitirAsync(long usuarioId);

        // Valida o token, revoga-o e emite um substituto (rotação). Retorna null
        // se o token não existe, expirou ou já foi revogado.
        Task<RotacaoRefresh> RotacionarAsync(string rawToken);

        // Revoga um token específico (logout).
        Task RevogarAsync(string rawToken);

        // Revoga todas as sessões de um usuário. Retorna quantas revogou.
        Task<int> RevogarUsuarioAsync(long usuarioId);
    }
}
