using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.DataAccess.Interfaces
{
    public interface IAlunoRepository : IBaseRepository<Aluno>
    {
        Task<Aluno> GetByNome(string nome);
        Task<List<Aluno>> SearchByNome(string nome);
    }
}
