using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Aviso/comunicado interno. Um administrador publica; os usuários do
    // público-alvo veem ao logar e marcam "ciente" (para de aparecer para eles)
    // ou apenas adiam (reaparece no próximo login — sem registro).
    public class Aviso : Base
    {
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public int PublicoAlvo { get; set; } // 0=Todos, 1=Professores, 2=Supervisores
        public DateTime DataCriacao { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Mensagem))
                _errors.Add("A mensagem do aviso é obrigatória.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
