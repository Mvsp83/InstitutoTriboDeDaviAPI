using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class PoloValidator : AbstractValidator<Polo>
    {
        public PoloValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("O Polo não pode ser vazio.")

                .NotNull()
                .WithMessage("O Polo não pode ser nulo.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O Nome não pode ser vazio.")

                .NotNull()
                .WithMessage("O Nome não pode ser nulo.")

                .MinimumLength(3)
                .WithMessage("O Nome deve ter no mínimo 3 caracteres.")

                .MaximumLength(120)
                .WithMessage("O Nome deve ter no máximo 120 caracteres.");
        }
    }
}
