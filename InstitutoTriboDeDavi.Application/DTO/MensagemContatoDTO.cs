namespace InstitutoTriboDeDavi.Application.DTO
{
    // Mensagem do Fale Conosco, como a equipe vê na caixa de entrada.
    public class MensagemContatoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Lida { get; set; }
    }

    // Payload público do formulário. Website é honeypot anti-spam (fica oculto no
    // formulário; se vier preenchido, é bot e a mensagem é descartada).
    public class EnviarContatoDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public string Website { get; set; }
    }
}
