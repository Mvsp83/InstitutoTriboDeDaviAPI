using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface ILogAuditoriaRepository
    {
        // Últimos registros, com filtros opcionais. `entidade` e `usuario`
        // ajudam a responder "o que mudou nesta tabela / quem fez".
        Task<List<LogAuditoria>> ListarAsync(string entidade, string usuario, int limite);
    }
}
