using FluentValidation;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Domain.Validators
{
    public class AtividadeValidator : AbstractValidator<Atividade>
    {
        public AtividadeValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("A Atividade não pode ser nula.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O Nome da atividade não pode ser vazio.")
                .MaximumLength(120)
                .WithMessage("O Nome da atividade deve ter no máximo 120 caracteres.");

            RuleFor(x => x.Tags)
                .MaximumLength(200)
                .WithMessage("As Tags devem ter no máximo 200 caracteres.");

            RuleFor(x => x.Principio)
                .MaximumLength(200)
                .WithMessage("O Princípio deve ter no máximo 200 caracteres.");

            RuleFor(x => x.ReferenciaBiblica)
                .MaximumLength(120)
                .WithMessage("A Referência Bíblica deve ter no máximo 120 caracteres.");

            RuleFor(x => x.VideoUrl)
                .MaximumLength(300)
                .WithMessage("O link do vídeo deve ter no máximo 300 caracteres.");

            // Apenas http/https: evita armazenar esquemas perigosos (javascript:, data:)
            // que virariam XSS ao serem renderizados como link no portal
            RuleFor(x => x.VideoUrl)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .When(x => !string.IsNullOrWhiteSpace(x.VideoUrl))
                .WithMessage("O link do vídeo deve ser uma URL http ou https válida.");
        }
    }
}
