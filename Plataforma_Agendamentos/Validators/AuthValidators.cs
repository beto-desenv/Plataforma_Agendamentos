using FluentValidation;
using Plataforma_Agendamentos.DTOs;

namespace Plataforma_Agendamentos.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome e obrigatorio")
            .MaximumLength(100).WithMessage("Nome deve ter no maximo 100 caracteres")
            .Matches(@"^[a-zA-Z\u00C0-\u017F\s]+$").WithMessage("Nome deve conter apenas letras e espacos");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email e obrigatorio")
            .EmailAddress().WithMessage("Email deve ter um formato valido")
            .MaximumLength(150).WithMessage("Email deve ter no maximo 150 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha e obrigatoria")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres")
            .MaximumLength(100).WithMessage("Senha deve ter no maximo 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Senha deve conter pelo menos uma letra minuscula, uma maiuscula e um numero");

        RuleFor(x => x.UserTypes)
            .NotEmpty().WithMessage("Tipo de usuario e obrigatorio")
            .Must(types => types.All(t => t == "cliente" || t == "prestador"))
            .WithMessage("Tipos de usuario devem ser 'cliente' ou 'prestador'")
            .Must(types => types.Distinct().Count() == types.Count)
            .WithMessage("Nao e possivel duplicar tipos de usuario");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email e obrigatorio")
            .EmailAddress().WithMessage("Email deve ter um formato valido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha e obrigatoria");
    }
}

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Slug)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug deve conter apenas letras minusculas, numeros e hifens")
            .MaximumLength(100)
            .WithMessage("Slug deve ter no maximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Slug));

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Nome de exibicao deve ter no maximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.DisplayName));

        RuleFor(x => x.PrimaryColor)
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("Cor primaria deve estar no formato hexadecimal (#RRGGBB)")
            .When(x => !string.IsNullOrEmpty(x.PrimaryColor));

        RuleFor(x => x.LogoUrl)
            .Must(BeAValidUrl).WithMessage("URL do logo deve ser uma URL valida")
            .When(x => !string.IsNullOrEmpty(x.LogoUrl));

        RuleFor(x => x.CoverImageUrl)
            .Must(BeAValidUrl).WithMessage("URL da capa deve ser uma URL valida")
            .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio deve ter no maximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Bio));
    }

    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) && 
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}