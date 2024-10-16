using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface IEstadoRepository : IBaseRepository<Estado>
    {
        Task<Estado> GetByNome(string nome);
        Task<List<Estado>> SearchByNome(string nome);
    }
}
