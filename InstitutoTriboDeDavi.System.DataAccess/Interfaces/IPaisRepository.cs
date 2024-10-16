using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface IPaisRepository : IBaseRepository<Pais>
    {
        Task<Pais> GetByNome(string nome);
        Task<List<Pais>> SearchByNome(string nome);
    }
}
