using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Queries;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IAniversarianteService
    {
        Task<List<AniversarianteDTO>> GetAniversariantesAsync(UsuarioDTO usuarioDTO, int mes);
    }
}
