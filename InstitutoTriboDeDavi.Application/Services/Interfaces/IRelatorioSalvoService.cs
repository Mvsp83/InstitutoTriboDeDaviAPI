using InstitutoTriboDeDavi.Application.DTO.Business;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IRelatorioSalvoService
    {
        Task<List<RelatorioSalvoDTO>> GetPorUsuario(string usuarioLogin);
        Task<RelatorioSalvoDTO> Create(RelatorioSalvoDTO relatorioDTO);
        Task Delete(long id, string usuarioLogin);
    }
}
