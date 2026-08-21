using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Lançamento de uma conta. O valor é sempre positivo; o sinal vem do Tipo
    // ("Credito" entra, "Debito" sai).
    public class MovimentacaoFinanceira : Base
    {
        public long ContaId { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        // Id da categoria do catálogo do portal (ex.: "doacoes", "aluguel").
        public string CategoriaId { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Debito";
        public decimal Valor { get; set; }
        public bool Conciliado { get; set; }
        public string Documento { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        // Liga os dois lados de uma transferência entre contas: o débito na
        // origem e o crédito no destino compartilham o mesmo identificador.
        public string TransferenciaId { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (ContaId <= 0)
                _errors.Add("O lançamento precisa estar vinculado a uma conta.");

            if (string.IsNullOrWhiteSpace(Descricao))
                _errors.Add("A descrição do lançamento é obrigatória.");
            else if (Descricao.Length > 200)
                _errors.Add("A descrição deve ter no máximo 200 caracteres.");

            if (string.IsNullOrWhiteSpace(CategoriaId))
                _errors.Add("A categoria do lançamento é obrigatória.");

            if (Tipo != "Credito" && Tipo != "Debito")
                _errors.Add("O tipo do lançamento é inválido.");

            // Valor negativo indicaria sinal duplicado (o sinal vem do Tipo).
            if (Valor <= 0)
                _errors.Add("O valor deve ser maior que zero.");

            if (Data == default)
                _errors.Add("A data do lançamento é obrigatória.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
