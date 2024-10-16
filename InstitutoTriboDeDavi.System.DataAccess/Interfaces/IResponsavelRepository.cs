using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface IResponsavelRepository : IBaseRepository<Responsavel>
    {
        Task<Responsavel> GetByNome(string nome);
        Task<List<Responsavel>> SearchByNome(string nome);
    }
}
