using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IGovernancaService
    {
        Task<List<MembroGovernancaDTO>> ListarPorAno(int ano);
        Task<List<int>> ListarAnos();
        Task Salvar(int ano, List<MembroGovernancaDTO> membros);

        // Público (Transparência): membros do ano pedido (ou o mais recente) +
        // os anos disponíveis para o seletor.
        Task<GovernancaPublicaDTO> ObterPublico(int? ano);
    }
}
