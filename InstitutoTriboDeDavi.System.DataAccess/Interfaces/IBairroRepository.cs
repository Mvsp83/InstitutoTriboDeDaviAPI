using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface IBairroRepository : IBaseRepository<Bairro>
    {
        Task<Bairro> GetByNome(string nome);
        Task<List<Bairro>> SearchByNome(string nome);
    }
}
