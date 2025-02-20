using FluentValidation;
using InstitutoTriboDeDavi.System.Domain.Entities;

namespace InstitutoTriboDeDavi.System.Domain.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("O Usuário não pode ser vazio.")

                .NotNull()
                .WithMessage("O Usuário não pode ser nulo.");

            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("O Nome não pode ser vazio.")

                .NotNull()
                .WithMessage("O Nome não pode ser nulo.")

                .MinimumLength(3)
                .WithMessage("O Login deve ter no mínimo 3 caracteres.")

                .MaximumLength(20)
                .WithMessage("O Login deve ter no máximo 20 caracteres.");

            RuleFor(x => x.SenhaHash)
                .NotEmpty()
                .WithMessage("O Password não pode ser vazio.")

                .NotNull()
                .WithMessage("O Password não pode ser nulo.")

                .MinimumLength(3)
                .WithMessage("O Password deve ter no mínimo 3 caracteres.")

                .MaximumLength(20)
                .WithMessage("O Password deve ter no máximo 20 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O Email não pode ser vazio.")

                .NotNull()
                .WithMessage("O Email não pode ser nulo.")

                .Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
                .WithMessage("O Email informado não é valido.");
        }
    }
}
