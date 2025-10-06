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
    public string? Slug { get; set; }
    public string? DisplayName { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? Bio { get; set; }
}