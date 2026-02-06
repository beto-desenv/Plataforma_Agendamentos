namespace Plataforma_Agendamentos.DTOs.Auth;

/// <summary>
/// DTO de usuario para resposta
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? DisplayName { get; set; }
}
