using System.Collections.Generic;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Plano de mensalidade cobrável (ex.: Integral, Social, Meia-bolsa). O
    // vencimento não é fixo no plano: ele oferece dias possíveis e o aluno
    // escolhe um na matrícula financeira.
    public class PlanoMensalidade : Base
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        // Dias de vencimento oferecidos (1..28). Vazio = qualquer dia.
        public List<int> OpcoesVencimento { get; set; } = new();
        public bool Ativo { get; set; } = true;
        public string Descricao { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("O nome do plano é obrigatório.");
            else if (Nome.Length > 120)
                _errors.Add("O nome do plano deve ter no máximo 120 caracteres.");

            if (Valor < 0)
                _errors.Add("O valor do plano não pode ser negativo.");

            if (OpcoesVencimento.Any(d => d < 1 || d > 28))
                _errors.Add("As opções de vencimento devem estar entre os dias 1 e 28.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
