using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface ICompeticaoEventoService
    {
        Task<List<CompeticaoEventoDTO>> Listar();
        Task<CompeticaoEventoDTO> Obter(long id);
        Task<CompeticaoEventoDTO> Criar(CompeticaoEventoDTO dto);
        Task<CompeticaoEventoDTO> Atualizar(CompeticaoEventoDTO dto);
        Task Remover(long id);

        Task<ParticipacaoAtletaDTO> AdicionarParticipacao(long eventoId, ParticipacaoAtletaDTO dto);
        Task<ParticipacaoAtletaDTO> AtualizarParticipacao(ParticipacaoAtletaDTO dto);
        Task RemoverParticipacao(long id);
    }
}
