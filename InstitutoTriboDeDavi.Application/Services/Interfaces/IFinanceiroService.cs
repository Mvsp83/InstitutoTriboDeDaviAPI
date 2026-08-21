using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IFinanceiroService
    {
        Task<List<ContaFinanceiraDTO>> ListarContas();
        Task<ContaFinanceiraDTO> SalvarConta(ContaFinanceiraDTO dto);
        Task ExcluirConta(long id);

        Task<List<MovimentacaoFinanceiraDTO>> ListarMovimentacoes();
        Task<MovimentacaoFinanceiraDTO> SalvarMovimentacao(MovimentacaoFinanceiraDTO dto);
        Task ExcluirMovimentacao(long id);
        Task DefinirConciliacao(long id, bool conciliado);
        Task RegistrarTransferencia(TransferenciaDTO dto);

        Task<ResultadoImportacaoDTO> Importar(ImportacaoFinanceiraDTO dto);
    }
}
