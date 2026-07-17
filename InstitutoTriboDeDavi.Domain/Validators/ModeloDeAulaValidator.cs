using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class ModeloDeAulaValidator : AbstractValidator<ModeloDeAula>
    {
        public ModeloDeAulaValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("O Modelo de Aula não pode ser nulo.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O Nome do modelo não pode ser vazio.")
                .MaximumLength(120)
                .WithMessage("O Nome do modelo deve ter no máximo 120 caracteres.");

            RuleFor(x => x.DuracaoTotalMinutos)
                .GreaterThan(0)
                .WithMessage("A Duração Total deve ser maior que zero.");

            RuleFor(x => x)
                .Must(x => x.Blocos == null || x.Blocos.Sum(b => b.DuracaoMinutos) <= x.DuracaoTotalMinutos)
                .WithMessage("A soma das durações dos blocos não pode ultrapassar a Duração Total do modelo.");

            RuleFor(x => x)
                .Must(x => x.Blocos == null || x.Blocos.Select(b => b.Ordem).Distinct().Count() == x.Blocos.Count)
                .WithMessage("Os blocos não podem ter Ordem repetida.");

            RuleForEach(x => x.Blocos)
                .SetValidator(new BlocoDoModeloValidator());
        }
    }
}
