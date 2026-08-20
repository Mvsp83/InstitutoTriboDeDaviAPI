using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Ofício ou Recibo com numeração oficial (AAAA/NNNN, reinicia a cada ano).
    // Fluxo: Rascunho (editável, sem número) -> Aprovado (número atribuído e
    // imutável). Conteudo guarda os campos específicos do tipo em JSON — o
    // portal é quem monta o PDF a partir dele.
    public class DocumentoOficial : Base
    {
        public int Tipo { get; set; }        // 0 = Ofício, 1 = Recibo
        public int Status { get; set; }      // 0 = Rascunho, 1 = Aprovado
        public int Ano { get; set; }
        public int Numero { get; set; }      // 0 enquanto rascunho
        public string NumeroFormatado { get; set; } = string.Empty;
        public DateTime DataDocumento { get; set; }
        public string Titulo { get; set; } = string.Empty;  // resumo p/ listagem
        public string Conteudo { get; set; } = string.Empty; // JSON dos campos
        public DateTime? DataAprovacao { get; set; }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                _errors.Add("Informe um título/identificação do documento.");
            if (DataDocumento == default)
                _errors.Add("Informe a data do documento.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
