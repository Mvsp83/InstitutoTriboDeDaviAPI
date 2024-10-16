using FluentValidation;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.Domain.Validators
{
    public class EnderecoValidator : AbstractValidator<Endereco>
    {
        public EnderecoValidator() 
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("O Pais não pode ser vazio.")

                .NotNull()
                .WithMessage("O Pais não pode ser nulo.");

            RuleFor(x => x.Logradouro)
                .NotEmpty()
                .WithMessage("O Logradouro não pode ser vazio.")

                .NotNull()
                .WithMessage("O Logradouro não pode ser nulo.")

                .MinimumLength(3)
                .WithMessage("O Logradouro deve ter no mínimo 3 caracteres.")

                .MaximumLength(120)
                .WithMessage("O Logradouro deve ter no máximo 120 caracteres.");
        }
    }
}
