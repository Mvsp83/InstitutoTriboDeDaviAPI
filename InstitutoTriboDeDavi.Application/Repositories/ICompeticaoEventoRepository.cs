using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface ICompeticaoEventoRepository
    {
        Task<List<CompeticaoEvento>> ListarAsync();
        Task<CompeticaoEvento> ObterComParticipacoesAsync(long id);
        Task<CompeticaoEvento> CriarAsync(CompeticaoEvento evento);
        Task AtualizarAsync(CompeticaoEvento evento);
        Task RemoverAsync(long id);

        Task<ParticipacaoAtleta> AdicionarParticipacaoAsync(ParticipacaoAtleta p);
        Task<ParticipacaoAtleta> AtualizarParticipacaoAsync(ParticipacaoAtleta p);
        Task<ParticipacaoAtleta> ObterParticipacaoAsync(long id);
        Task RemoverParticipacaoAsync(long id);
    }
}
