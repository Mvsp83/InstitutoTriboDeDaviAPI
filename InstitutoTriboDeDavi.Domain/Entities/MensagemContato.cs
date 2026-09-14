using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Mensagem enviada pelo "Fale Conosco" público. Fica na caixa de entrada que
    // a equipe lê no admin; a notificação por e-mail é best-effort.
    public class MensagemContato : Base
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public bool Lida { get; set; }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("Informe seu nome.");
            if (string.IsNullOrWhiteSpace(Mensagem))
                _errors.Add("Escreva sua mensagem.");
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Telefone))
                _errors.Add("Informe um e-mail ou telefone para retorno.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
