using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Cobrança (mensalidade) de um aluno numa competência. O valor é congelado
    // na geração (já com o desconto), para não mudar se o plano for reajustado.
    // "atrasado" não é um status: é derivado (pendente + vencimento passado).
    public class Cobranca : Base
    {
        public long AlunoId { get; set; }
        public long? PlanoId { get; set; }
        public string Competencia { get; set; } = string.Empty; // "yyyy-MM"
        public DateTime Vencimento { get; set; }
        public decimal Valor { get; set; }
        // "pendente", "pago", "cancelado" ou "isento".
        public string Status { get; set; } = "pendente";
        public DateTime? PagamentoData { get; set; }
        public decimal? PagamentoValor { get; set; }
        public string PagamentoForma { get; set; } = string.Empty;
        // Integração com o livro-caixa: conta que recebeu e lançamento gerado.
        public long? ContaId { get; set; }
        public long? MovimentacaoId { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (AlunoId <= 0)
                _errors.Add("A cobrança precisa estar vinculada a um aluno.");

            if (string.IsNullOrWhiteSpace(Competencia))
                _errors.Add("A competência da cobrança é obrigatória.");

            if (Valor < 0)
                _errors.Add("O valor da cobrança não pode ser negativo.");

            if (Status != "pendente" && Status != "pago"
                && Status != "cancelado" && Status != "isento")
                _errors.Add("O status da cobrança é inválido.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
