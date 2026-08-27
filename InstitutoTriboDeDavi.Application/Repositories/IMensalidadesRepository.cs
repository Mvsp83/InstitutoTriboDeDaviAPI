using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IMensalidadesRepository
    {
        // Planos
        Task<List<PlanoMensalidade>> ListarPlanosAsync();
        Task<PlanoMensalidade> ObterPlanoAsync(long id);
        Task<PlanoMensalidade> SalvarPlanoAsync(PlanoMensalidade plano);
        Task ExcluirPlanoAsync(long id);

        // Matrículas financeiras
        Task<List<MatriculaFinanceira>> ListarMatriculasAsync();
        Task<MatriculaFinanceira> SalvarMatriculaAsync(MatriculaFinanceira matricula);
        Task ExcluirMatriculaAsync(long id);
        Task<List<MatriculaFinanceira>> ListarMatriculasAtivasAsync();

        // Cobranças
        Task<List<Cobranca>> ListarCobrancasAsync(string competencia);
        Task<Cobranca> ObterCobrancaAsync(long id);
        Task<Cobranca> SalvarCobrancaAsync(Cobranca cobranca);
        Task ExcluirCobrancaAsync(long id);
        // Ids de aluno que já têm cobrança na competência (para não duplicar).
        Task<HashSet<long>> AlunosComCobrancaAsync(string competencia);
        // Insere em lote as cobranças geradas.
        Task<int> AdicionarCobrancasAsync(List<Cobranca> cobrancas);

        // Baixa: marca como paga e cria a movimentação de receita no livro-caixa
        // (categoria "mensalidades"), tudo numa transação. Devolve a cobrança.
        Task<Cobranca> BaixarAsync(
            long id, System.DateTime pagamentoData, decimal pagamentoValor,
            string pagamentoForma, long contaId);
    }
}
