using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Queries;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IFrequenciaService
    {
        Task<List<FrequenciaDTO>> GetAlunosFaltasAsync(UsuarioDTO usuarioDTO);
    }
}
