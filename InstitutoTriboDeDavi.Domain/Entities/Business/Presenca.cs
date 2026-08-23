using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    public class Presenca : Base
    {
        public long AlunoId { get; set; }
        public string NomeAluno { get; set; }
        public long PoloId { get; set; }
        public DateTime Data { get; set; }
        public bool EstaPresente { get; set; }
        public string Observacoes { get; set; }
        public long AulaId { get; set; }

        // Justificativa da falta enviada pelo responsável no portal. Nula
        // enquanto a falta não é justificada; só faz sentido quando
        // EstaPresente == false.
        public string JustificativaResponsavel { get; set; }
        public DateTime? JustificadaEm { get; set; }

        public override bool Validate()
        {
            var validator = new PresencaValidator();
            var validation = validator.Validate(this);

            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    _errors.Add(error.ErrorMessage);
                }

                throw new DomainException("Alguns campos estão inválidos!", _errors);
            }

            return true;
        }
    }
}
