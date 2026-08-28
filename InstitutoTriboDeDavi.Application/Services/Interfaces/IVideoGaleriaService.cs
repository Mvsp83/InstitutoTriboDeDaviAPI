using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IVideoGaleriaService
    {
        Task<List<VideoGaleriaDTO>> Listar();
        Task<VideoGaleriaDTO> Salvar(VideoGaleriaDTO dto);
        Task Excluir(long id);
    }
}
