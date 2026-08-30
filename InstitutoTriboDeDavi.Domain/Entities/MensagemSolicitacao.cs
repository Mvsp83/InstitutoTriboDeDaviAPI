using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Uma mensagem da conversa de uma SolicitacaoInterna (o corpo inicial ou uma
    // resposta). Ordenadas por DataEnvio para montar a thread.
    public class MensagemSolicitacao : Base
    {
        public long SolicitacaoInternaId { get; set; }
        public string AutorLogin { get; set; } = string.Empty;
        public int AutorRole { get; set; } // UserRole
        public string Texto { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Texto))
                _errors.Add("A mensagem não pode ficar vazia.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
