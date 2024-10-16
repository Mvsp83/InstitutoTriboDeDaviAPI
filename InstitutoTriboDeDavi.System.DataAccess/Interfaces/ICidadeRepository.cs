using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface ICidadeRepository : IBaseRepository<Cidade>
    {
        Task<Cidade> GetByNome(string nome);
        Task<List<Cidade>> SearchByNome(string nome);
    }
}
