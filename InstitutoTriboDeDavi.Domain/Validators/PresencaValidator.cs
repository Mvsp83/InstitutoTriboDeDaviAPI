using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class PresencaValidator : AbstractValidator<Presenca>
    {
        public PresencaValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("A Presença não pode ser nula.");

            RuleFor(x => x.AlunoId)
                .GreaterThan(0)
                .WithMessage("O AlunoId deve ser maior que zero.");

            RuleFor(x => x.PoloId)
                .GreaterThan(0)
                .WithMessage("O PoloId deve ser maior que zero.");

            RuleFor(x => x.AulaId)
                .GreaterThan(0)
                .WithMessage("O AulaId deve ser maior que zero.");

            RuleFor(x => x.Data)
                .NotEmpty()
                .WithMessage("A Data da presença não pode ser vazia.");
        }
    }
}
