using FluentValidation;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.Domain.Validators
{
    public class EstadoValidator : AbstractValidator<Estado>
    {
        public EstadoValidator() 
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("O Pais não pode ser vazio.")

                .NotNull()
                .WithMessage("O Pais não pode ser nulo.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O Nome não pode ser vazio.")

                .NotNull()
                .WithMessage("O Nome não pode ser nulo.")

                .MinimumLength(3)
                .WithMessage("O Nome deve ter no mínimo 3 caracteres.")

                .MaximumLength(120)
                .WithMessage("O Nome deve ter no máximo 120 caracteres.");

            RuleFor(x => x.Sigla)
                .NotEmpty()
                .WithMessage("A Sigla não pode ser vazia.")

                .NotNull()
                .WithMessage("A Sigla não pode ser nula.")

                .MinimumLength(2)
                .WithMessage("A Sigla deve ter no mínimo 2 caracteres.")

                .MaximumLength(2)
                .WithMessage("A Sigla deve ter no máximo 2 caracteres.");
        }
    }
}
