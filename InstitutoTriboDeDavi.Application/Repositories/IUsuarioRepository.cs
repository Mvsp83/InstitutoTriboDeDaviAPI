using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario> GetByEmail(string email);
        Task<Usuario> GetByNome(string nome);
        Task<List<Usuario>> SearchByEmail(string email);
        Task<List<Usuario>> SearchByNome(string nome);
        Task<Usuario> ObterUsuarioPorLoginAsync(string login);
        Task<List<Usuario>> ObterTodosAsync();
        Task<List<Usuario>> ObterPorPoloTurmaAsync(long poloId, List<int> turmas);
        Task<bool> ExisteQualquerUsuario();
    }
}
