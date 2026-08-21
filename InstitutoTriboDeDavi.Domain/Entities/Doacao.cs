using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Uma doação recebida. DoadorId nulo é doação anônima — comum no Pix, e por
    // isso não é obrigatório: registrar o valor recebido importa mais do que
    // saber de quem veio.
    public class Doacao : Base
    {
        public long? DoadorId { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        // "Pix", "Dinheiro", "Transferencia", "Outro".
        public string Forma { get; set; } = "Pix";
        // Campanha ou finalidade (ex.: "Kimonos 2026"). Vazio = uso geral.
        public string Finalidade { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        // Recibo emitido para esta doação (DocumentoOficial). Nulo = sem recibo.
        public long? ReciboDocumentoId { get; set; }
        public string ReciboNumero { get; set; } = string.Empty;
        public string RegistradoPor { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (Valor <= 0)
                _errors.Add("O valor da doação deve ser maior que zero.");

            if (Data == default)
                _errors.Add("A data da doação é obrigatória.");
            else if (Data.Date > DateTime.Today)
                _errors.Add("A data da doação não pode estar no futuro.");

            var formas = new[] { "Pix", "Dinheiro", "Transferencia", "Outro" };
            if (!formas.Contains(Forma))
                _errors.Add("A forma de recebimento é inválida.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
