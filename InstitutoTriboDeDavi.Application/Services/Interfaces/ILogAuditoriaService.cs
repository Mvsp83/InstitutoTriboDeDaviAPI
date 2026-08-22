using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface ILogAuditoriaService
    {
        Task<List<LogAuditoriaDTO>> Listar(string entidade, string usuario, int limite);
    }
}
