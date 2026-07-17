using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class PlanoDeAulaValidator : AbstractValidator<PlanoDeAula>
    {
        public PlanoDeAulaValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("O Plano de Aula não pode ser nulo.");

            RuleFor(x => x.PoloId)
                .GreaterThan(0)
                .WithMessage("O PoloId deve ser maior que zero.");

            RuleFor(x => x.Titulo)
                .NotEmpty()
                .WithMessage("O Título do plano não pode ser vazio.")
                .MaximumLength(120)
                .WithMessage("O Título do plano deve ter no máximo 120 caracteres.");

            RuleFor(x => x.DataPrevista)
                .NotEmpty()
                .WithMessage("A Data Prevista do plano não pode ser vazia.");

            RuleFor(x => x.DuracaoTotalMinutos)
                .GreaterThan(0)
                .WithMessage("A Duração Total deve ser maior que zero.");

            RuleFor(x => x)
                .Must(x => x.Blocos == null || x.Blocos.Sum(b => b.DuracaoMinutos) <= x.DuracaoTotalMinutos)
                .WithMessage("A soma das durações dos blocos não pode ultrapassar a Duração Total da aula.");

            RuleFor(x => x)
                .Must(x => x.Blocos == null || x.Blocos.Select(b => b.Ordem).Distinct().Count() == x.Blocos.Count)
                .WithMessage("Os blocos não podem ter Ordem repetida.");

            RuleForEach(x => x.Blocos)
                .SetValidator(new BlocoDoPlanoValidator());
        }
    }
}
