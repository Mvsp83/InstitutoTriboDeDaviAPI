using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IEventoCalendarioRepository : IBaseRepository<EventoCalendario>
    {
        Task<List<EventoCalendario>> ObterPorAnoAsync(int ano);
        Task<List<int>> ObterAnosAsync();
        Task CriarVariosAsync(IEnumerable<EventoCalendario> eventos);
        // Eventos com notificação ligada e ainda não enviada (para o job diário).
        Task<List<EventoCalendario>> ObterPendentesNotificacaoAsync();
        Task MarcarNotificadaAsync(long id);
    }
}
