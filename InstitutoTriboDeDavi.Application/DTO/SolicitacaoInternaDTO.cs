namespace InstitutoTriboDeDavi.Application.DTO
{
    // Leitura: solicitação com a conversa completa (usada no detalhe) ou sem
    // (na listagem, quando Mensagens vem vazia).
    public class SolicitacaoInternaDTO
    {
        public long Id { get; set; }
        public string Assunto { get; set; }
        public int Categoria { get; set; }
        public int Status { get; set; }
        public long? PoloId { get; set; }
        public string PoloNome { get; set; }
        public string CriadoPorLogin { get; set; }
        public int CriadoPorRole { get; set; }
        public string DestinatarioLogin { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
        public List<MensagemSolicitacaoDTO> Mensagens { get; set; } = new();
    }

    public class MensagemSolicitacaoDTO
    {
        public long Id { get; set; }
        public long SolicitacaoInternaId { get; set; }
        public string AutorLogin { get; set; }
        public int AutorRole { get; set; }
        public string Texto { get; set; }
        public DateTime DataEnvio { get; set; }
    }

    // Escrita: abre uma solicitação. O professor deixa Destinatario/Polo nulos
    // (vai para a administração, com o polo dele); o admin informa o professor
    // alvo (DestinatarioLogin) e o polo correspondente.
    public class CriarSolicitacaoDTO
    {
        public string Assunto { get; set; }
        public int Categoria { get; set; }
        public string Texto { get; set; } // corpo inicial da solicitação
        public string DestinatarioLogin { get; set; }
        public long? PoloId { get; set; }
        public string PoloNome { get; set; }
    }

    public class ResponderSolicitacaoDTO
    {
        public string Texto { get; set; }
    }

    public class AlterarStatusSolicitacaoDTO
    {
        public int Status { get; set; }
    }
}
