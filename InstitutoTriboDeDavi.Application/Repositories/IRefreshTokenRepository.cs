using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CriarAsync(RefreshToken token);
        // Busca pelo hash; retorna null se não existe.
        Task<RefreshToken> ObterPorHashAsync(string tokenHash);
        // Marca um token como revogado (rotação ou logout).
        Task RevogarAsync(RefreshToken token);
        // Revoga todos os tokens ativos de um usuário ("sair de todos os
        // aparelhos"; desativar/excluir usuário). Retorna quantos revogou.
        Task<int> RevogarTodosDoUsuarioAsync(long usuarioId);
    }
}
