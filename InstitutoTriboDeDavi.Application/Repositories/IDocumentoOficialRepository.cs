using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IDocumentoOficialRepository : IBaseRepository<DocumentoOficial>
    {
        Task<List<DocumentoOficial>> ObterPorAnoAsync(int ano);
        Task<List<int>> ObterAnosAsync();
        // Aprova o documento atribuindo o próximo número oficial do tipo/ano,
        // em transação serializável (evita numeração duplicada).
        Task<DocumentoOficial> AprovarAsync(long id);
    }
}
