using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IRelatorioSalvoRepository : IBaseRepository<RelatorioSalvo>
    {
        Task<List<RelatorioSalvo>> GetByUsuarioAsync(string usuarioLogin);
    }
}
