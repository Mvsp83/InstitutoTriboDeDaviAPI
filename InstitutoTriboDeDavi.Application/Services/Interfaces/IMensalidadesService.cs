using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IMensalidadesService
    {
        // Planos
        Task<List<PlanoMensalidadeDTO>> ListarPlanos();
        Task<PlanoMensalidadeDTO> SalvarPlano(PlanoMensalidadeDTO dto);
        Task ExcluirPlano(long id);

        // Matrículas
        Task<List<MatriculaFinanceiraDTO>> ListarMatriculas();
        Task<MatriculaFinanceiraDTO> SalvarMatricula(MatriculaFinanceiraDTO dto);
        Task ExcluirMatricula(long id);

        // Cobranças
        Task<List<CobrancaDTO>> ListarCobrancas(string competencia);
        Task<ResultadoGeracaoDTO> GerarCobrancas(string competencia);
        Task<CobrancaDTO> Baixar(BaixaCobrancaDTO dto);
        Task<CobrancaDTO> SalvarCobranca(CobrancaDTO dto);
        Task ExcluirCobranca(long id);
    }
}
