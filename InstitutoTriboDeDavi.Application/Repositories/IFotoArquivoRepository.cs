using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IFotoArquivoRepository
    {
        Task<FotoArquivo> AdicionarAsync(FotoArquivo arquivo);
        Task<FotoArquivo> ObterAsync(long id);
        Task ExcluirAsync(long id);
        // Guarda a miniatura gerada sob demanda para esta foto.
        Task SalvarMiniaturaAsync(long id, byte[] miniatura);
    }
}
