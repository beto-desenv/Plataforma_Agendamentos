using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public List<string> UserTypes { get; set; } = new List<string>(); // ["cliente", "prestador"]
}

public class AddRoleRequest
{
    [Required]
    public string Role { get; set; } = string.Empty; // "cliente" ou "prestador"
}

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
