namespace Plataforma_Agendamentos.DTOs.Auth;

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
