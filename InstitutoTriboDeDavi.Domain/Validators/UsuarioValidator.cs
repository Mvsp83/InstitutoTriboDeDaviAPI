using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Domain.Validators
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

            // Valida o hash gerado (o tamanho da senha em texto é validado no service,
            // pois o hash do Identity tem ~84 caracteres)
            RuleFor(x => x.SenhaHash)
                .NotEmpty()
                .WithMessage("A Senha não pode ser vazia.")

                .NotNull()
                .WithMessage("A Senha não pode ser nula.");

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
