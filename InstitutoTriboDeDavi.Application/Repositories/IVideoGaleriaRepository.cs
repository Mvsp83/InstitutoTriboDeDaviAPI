using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IVideoGaleriaRepository
    {
        Task<List<VideoGaleria>> ListarAsync();
        Task<VideoGaleria> SalvarAsync(VideoGaleria video);
        Task ExcluirAsync(long id);
    }
}
