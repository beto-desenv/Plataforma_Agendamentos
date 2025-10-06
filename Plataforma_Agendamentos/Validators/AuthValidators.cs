using FluentValidation;
using Plataforma_Agendamentos.DTOs;

namespace Plataforma_Agendamentos.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter um formato válido")
            .MaximumLength(150).WithMessage("Email deve ter no máximo 150 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres")
            .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Senha deve conter pelo menos uma letra minúscula, uma maiúscula e um número");

        RuleFor(x => x.UserTypes)
            .NotEmpty().WithMessage("Tipo de usuário é obrigatório")
            .Must(types => types.All(t => t == "cliente" || t == "prestador"))
            .WithMessage("Tipos de usuário devem ser 'cliente' ou 'prestador'")
            .Must(types => types.Distinct().Count() == types.Count)
            .WithMessage("Não é possível duplicar tipos de usuário");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter um formato válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória");
    }
}

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Slug)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug deve conter apenas letras minúsculas, números e hífens")
            .MaximumLength(100)
            .WithMessage("Slug deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Slug));

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Nome de exibição deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.DisplayName));

        RuleFor(x => x.PrimaryColor)
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("Cor primária deve estar no formato hexadecimal (#RRGGBB)")
            .When(x => !string.IsNullOrEmpty(x.PrimaryColor));

        RuleFor(x => x.LogoUrl)
            .Must(BeAValidUrl).WithMessage("URL do logo deve ser uma URL válida")
            .When(x => !string.IsNullOrEmpty(x.LogoUrl));

        RuleFor(x => x.CoverImageUrl)
            .Must(BeAValidUrl).WithMessage("URL da capa deve ser uma URL válida")
            .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Bio));
    }

    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) && 
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}