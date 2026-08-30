using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IAtletaRepository
    {
        Task<List<Atleta>> ListarAsync();
        Task<Atleta> ObterComTudoAsync(long id);
        Task<Atleta> ObterPorAlunoAsync(long alunoId);
        Task<Atleta> CriarAsync(Atleta atleta);
        Task AtualizarPerfilAsync(Atleta atleta);
        Task RemoverAsync(long id);

        // Filhos: adicionar e remover.
        Task<AvaliacaoFisica> AdicionarAvaliacaoAsync(AvaliacaoFisica avaliacao);
        Task RemoverAvaliacaoAsync(long id);
        Task<Competicao> AdicionarCompeticaoAsync(Competicao competicao);
        Task RemoverCompeticaoAsync(long id);
        Task<AnotacaoAtleta> AdicionarAnotacaoAsync(AnotacaoAtleta anotacao);
        Task RemoverAnotacaoAsync(long id);
        Task<MetaAtleta> AdicionarMetaAsync(MetaAtleta meta);
        Task<MetaAtleta> AtualizarMetaAsync(long id, int status);
        Task RemoverMetaAsync(long id);
        Task<Lesao> AdicionarLesaoAsync(Lesao lesao);
        Task<Lesao> MarcarRecuperadaAsync(long id, bool recuperado);
        Task RemoverLesaoAsync(long id);
    }
}
