using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Queries;

namespace InstitutoTriboDeDavi.System.Services.Business.Interfaces
{
    public interface IAniversarianteService
    {
        Task<List<AniversarianteDTO>> GetAniversariantesAsync(UsuarioDTO usuarioDTO, int mes);
    }
}
