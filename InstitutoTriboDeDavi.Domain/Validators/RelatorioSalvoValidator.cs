using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class RelatorioSalvoValidator : AbstractValidator<RelatorioSalvo>
    {
        public RelatorioSalvoValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("O Relatório não pode ser nulo.");

            RuleFor(x => x.UsuarioLogin)
                .NotEmpty()
                .WithMessage("O usuário do relatório não pode ser vazio.")
                .MaximumLength(120)
                .WithMessage("O usuário do relatório deve ter no máximo 120 caracteres.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O Nome do relatório não pode ser vazio.")
                .MaximumLength(120)
                .WithMessage("O Nome do relatório deve ter no máximo 120 caracteres.");

            RuleFor(x => x.FonteId)
                .NotEmpty()
                .WithMessage("A fonte de dados do relatório não pode ser vazia.")
                .MaximumLength(40)
                .WithMessage("A fonte de dados deve ter no máximo 40 caracteres.");

            RuleFor(x => x.Colunas)
                .NotEmpty()
                .WithMessage("Selecione ao menos uma coluna.")
                .MaximumLength(1000)
                .WithMessage("As colunas devem ter no máximo 1000 caracteres.");
        }
    }
}
