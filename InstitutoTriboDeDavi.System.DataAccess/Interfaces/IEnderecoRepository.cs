using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface IEnderecoRepository : IBaseRepository<Endereco>
    {
        Task<Endereco> GetByNome(string nome);
        Task<List<Endereco>> SearchByNome(string nome);
    }
}
