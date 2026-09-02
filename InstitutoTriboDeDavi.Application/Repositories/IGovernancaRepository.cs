using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IGovernancaRepository
    {
        Task<List<MembroGovernanca>> ListarPorAnoAsync(int ano);
        Task<List<int>> ListarAnosAsync();
        // Substitui todos os membros do ano pela lista informada (transação).
        Task SubstituirAnoAsync(int ano, List<MembroGovernanca> membros);
    }
}
