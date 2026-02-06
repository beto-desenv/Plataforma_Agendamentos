namespace Plataforma_Agendamentos.DTOs;

/// <summary>
/// Resultado de operacao de autenticacao (registro ou login)
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
}

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
