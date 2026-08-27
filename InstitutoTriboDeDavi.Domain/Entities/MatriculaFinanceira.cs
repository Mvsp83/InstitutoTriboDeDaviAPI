using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Vínculo financeiro de um aluno a um plano de mensalidade, com o dia de
    // vencimento escolhido e eventual bolsa/desconto. Não confundir com a
    // entidade Matricula (matrícula do aluno no ano letivo).
    public class MatriculaFinanceira : Base
    {
        public long AlunoId { get; set; }
        public long PlanoId { get; set; }
        // Dia de vencimento escolhido pelo aluno (dentre as opções do plano).
        public int DiaVencimento { get; set; }
        // Competência inicial "yyyy-MM": a partir de quando gera cobrança.
        public string Inicio { get; set; } = string.Empty;
        // "ativo", "suspenso" ou "encerrado".
        public string Status { get; set; } = "ativo";
        // "nenhum", "percentual", "valor" ou "isencao".
        public string DescontoTipo { get; set; } = "nenhum";
        public decimal DescontoValor { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (AlunoId <= 0)
                _errors.Add("A matrícula precisa estar vinculada a um aluno.");

            if (PlanoId <= 0)
                _errors.Add("A matrícula precisa estar vinculada a um plano.");

            if (DiaVencimento < 1 || DiaVencimento > 28)
                _errors.Add("O dia de vencimento deve estar entre 1 e 28.");

            if (Status != "ativo" && Status != "suspenso" && Status != "encerrado")
                _errors.Add("O status da matrícula é inválido.");

            if (DescontoTipo != "nenhum" && DescontoTipo != "percentual"
                && DescontoTipo != "valor" && DescontoTipo != "isencao")
                _errors.Add("O tipo de desconto é inválido.");

            if (DescontoValor < 0)
                _errors.Add("O valor do desconto não pode ser negativo.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
