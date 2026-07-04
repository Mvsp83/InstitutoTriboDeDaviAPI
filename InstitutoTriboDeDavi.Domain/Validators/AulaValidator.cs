using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class AulaValidator : AbstractValidator<Aula>
    {
        public AulaValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("A Aula não pode ser nula.");

            RuleFor(x => x.PoloId)
                .GreaterThan(0)
                .WithMessage("O PoloId deve ser maior que zero.");

            RuleFor(x => x.Data)
                .NotEmpty()
                .WithMessage("A Data da aula não pode ser vazia.");

            RuleFor(x => x.HoraInicio)
                .NotEmpty()
                .WithMessage("A Hora de Início não pode ser vazia.");

            RuleFor(x => x.HoraFim)
                .NotEmpty()
                .WithMessage("A Hora de Fim não pode ser vazia.");

            RuleFor(x => x)
                .Must(x => x.HoraFim > x.HoraInicio)
                .WithMessage("A Hora de Fim deve ser maior que a Hora de Início.");

            RuleFor(x => x.Turma)
                .GreaterThan(0)
                .WithMessage("A Turma deve ser maior que zero.");
        }
    }
}
