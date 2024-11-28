using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces
{
    public interface IPresencaRepository : IBaseRepository<Presenca>
    {
        Task<List<Presenca>> GetPresencasPorAula(long aulaId);
    }
}
