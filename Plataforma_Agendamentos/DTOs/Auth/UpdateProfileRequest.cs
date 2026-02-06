using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs.Auth;

public class UpdateProfileRequest
{
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens.")]
    public string? Slug { get; set; }

    [MaxLength(100)]
    public string? DisplayName { get; set; }

    [Url]
    public string? LogoUrl { get; set; }

    [Url]
    public string? CoverImageUrl { get; set; }

    [MaxLength(10)]
    [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "PrimaryColor deve ser um HEX válido.")]
    public string? PrimaryColor { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }
}
