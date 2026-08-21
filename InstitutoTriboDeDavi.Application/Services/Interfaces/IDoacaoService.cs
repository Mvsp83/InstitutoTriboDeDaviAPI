using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IDoacaoService
    {
        Task<List<DoadorDTO>> ListarDoadores();
        Task<DoadorDTO> SalvarDoador(DoadorDTO dto);
        Task ExcluirDoador(long id);

        Task<List<DoacaoDTO>> ListarDoacoes(int? ano, long? doadorId);
        Task<DoacaoDTO> SalvarDoacao(DoacaoDTO dto, string registradoPor);
        Task ExcluirDoacao(long id);
        Task<ResumoDoacoesDTO> Resumo(int ano);

        // Emite o recibo oficial numerado da doação.
        Task<DoacaoDTO> EmitirRecibo(long doacaoId);
    }
}
