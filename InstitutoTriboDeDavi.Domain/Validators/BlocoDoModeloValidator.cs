using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class BlocoDoModeloValidator : AbstractValidator<BlocoDoModelo>
    {
        public BlocoDoModeloValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("O Bloco do modelo não pode ser nulo.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O Nome do bloco não pode ser vazio.")
                .MaximumLength(80)
                .WithMessage("O Nome do bloco deve ter no máximo 80 caracteres.");

            RuleFor(x => x.DuracaoMinutos)
                .GreaterThan(0)
                .WithMessage("A Duração do bloco deve ser maior que zero.");

            RuleFor(x => x.Ordem)
                .GreaterThanOrEqualTo(0)
                .WithMessage("A Ordem do bloco não pode ser negativa.");
        }
    }
}
