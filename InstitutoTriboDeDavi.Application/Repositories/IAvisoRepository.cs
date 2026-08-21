using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IAvisoRepository : IBaseRepository<Aviso>
    {
        Task<List<Aviso>> ObterTodosAsync();
        Task<List<Aviso>> ObterAtivosAsync();
        // Ids dos avisos que o usuário já marcou como ciente.
        Task<List<long>> ObterCientesDoUsuarioAsync(string login);
        Task RegistrarCienteAsync(long avisoId, string login);
    }
}
