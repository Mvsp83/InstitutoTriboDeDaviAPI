using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IAtletaService
    {
        Task<List<AtletaDTO>> Listar();
        Task<AtletaDTO> Obter(long id);
        Task<AtletaDTO> Criar(long alunoId);
        Task<AtletaDTO> AtualizarPerfil(AtletaDTO dto);
        Task Remover(long id);

        Task<AvaliacaoFisicaDTO> AdicionarAvaliacao(long atletaId, AvaliacaoFisicaDTO dto);
        Task RemoverAvaliacao(long id);

        Task<CompeticaoDTO> AdicionarCompeticao(long atletaId, CompeticaoDTO dto);
        Task RemoverCompeticao(long id);

        Task<AnotacaoAtletaDTO> AdicionarAnotacao(long atletaId, string texto, string autor);
        Task RemoverAnotacao(long id);

        Task<MetaAtletaDTO> AdicionarMeta(long atletaId, MetaAtletaDTO dto);
        Task<MetaAtletaDTO> AlterarStatusMeta(long id, int status);
        Task RemoverMeta(long id);

        Task<LesaoDTO> AdicionarLesao(long atletaId, LesaoDTO dto);
        Task<LesaoDTO> MarcarLesaoRecuperada(long id, bool recuperado);
        Task RemoverLesao(long id);
    }
}
