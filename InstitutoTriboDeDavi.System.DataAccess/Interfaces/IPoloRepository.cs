using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface IPoloRepository : IBaseRepository<Polo>
    {
        Task<Polo> GetByNome(string nome);
        Task<List<Polo>> SearchByNome(string nome);        
    }
}
