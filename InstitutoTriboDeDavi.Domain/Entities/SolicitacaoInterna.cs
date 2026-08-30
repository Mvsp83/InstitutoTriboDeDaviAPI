using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Solicitação/comunicação interna entre a equipe. Duas vias:
    //  - Professor -> Administração: DestinatarioLogin nulo (vai para os admins).
    //  - Administração -> Professor: DestinatarioLogin é o login do professor alvo.
    // É um "chamado": tem categoria, status e uma conversa (Mensagens). A primeira
    // mensagem é o corpo do pedido; as demais são as respostas de cada lado.
    public class SolicitacaoInterna : Base
    {
        public string Assunto { get; set; } = string.Empty;
        public int Categoria { get; set; } // CategoriaSolicitacao
        public int Status { get; set; }    // StatusSolicitacao

        // Contexto do polo (do professor que abriu, ou do professor alvo).
        public long? PoloId { get; set; }
        public string PoloNome { get; set; } = string.Empty;

        // Quem abriu.
        public string CriadoPorLogin { get; set; } = string.Empty;
        public int CriadoPorRole { get; set; } // UserRole

        // Destinatário quando é a administração que dispara para um professor.
        // Nulo = solicitação dirigida à administração.
        public string? DestinatarioLogin { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;

        public List<MensagemSolicitacao> Mensagens { get; set; } = new();

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Assunto))
                _errors.Add("O assunto da solicitação é obrigatório.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
