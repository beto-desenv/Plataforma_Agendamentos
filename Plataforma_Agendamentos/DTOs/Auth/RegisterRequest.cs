using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs.Auth;

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
    public List<string> UserTypes { get; set; } = new List<string>();
}
