using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Conta bancária ou aplicação do instituto. O saldo não é guardado: ele é
    // sempre o saldo inicial somado às movimentações, para não haver divergência
    // entre um total gravado e os lançamentos que o formam.
    public class ContaFinanceira : Base
    {
        // "Corrente", "Poupanca" ou "Aplicacao" (mesmos valores do portal).
        public string Tipo { get; set; } = "Corrente";
        public string Nome { get; set; } = string.Empty;
        public string Banco { get; set; } = string.Empty;
        public string Agencia { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public bool Ativa { get; set; } = true;
        public string Observacoes { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("O nome da conta é obrigatório.");
            else if (Nome.Length > 160)
                _errors.Add("O nome da conta deve ter no máximo 160 caracteres.");

            if (Tipo != "Corrente" && Tipo != "Poupanca" && Tipo != "Aplicacao")
                _errors.Add("O tipo da conta é inválido.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
