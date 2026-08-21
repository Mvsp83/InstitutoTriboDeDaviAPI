using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IFinanceiroRepository
    {
        Task<List<ContaFinanceira>> ListarContasAsync();
        Task<ContaFinanceira> ObterContaAsync(long id);
        Task<ContaFinanceira> SalvarContaAsync(ContaFinanceira conta);
        // Excluir a conta remove junto os lançamentos dela (não deixa órfãos).
        Task ExcluirContaAsync(long id);

        Task<List<MovimentacaoFinanceira>> ListarMovimentacoesAsync();
        Task<MovimentacaoFinanceira> ObterMovimentacaoAsync(long id);
        Task<MovimentacaoFinanceira> SalvarMovimentacaoAsync(MovimentacaoFinanceira mov);
        // Excluir um lado de uma transferência remove o par junto.
        Task ExcluirMovimentacaoAsync(long id);
        Task DefinirConciliacaoAsync(long id, bool conciliado);

        // Grava os dois lançamentos da transferência numa única transação.
        Task RegistrarTransferenciaAsync(MovimentacaoFinanceira debito, MovimentacaoFinanceira credito);

        Task<bool> ExisteAlgumDadoAsync();
        // Importa a carga do navegador preservando o vínculo conta↔lançamento.
        Task<(int contas, int movimentacoes)> ImportarAsync(
            List<(ContaFinanceira conta, long idOrigem)> contas,
            List<(MovimentacaoFinanceira mov, long contaIdOrigem)> movimentacoes);
    }
}
