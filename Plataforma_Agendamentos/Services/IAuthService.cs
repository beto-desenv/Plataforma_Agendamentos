using Plataforma_Agendamentos.DTOs.Auth;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Interface para servico de autenticacao e registro de usuarios
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registra um novo usuario no sistema
    /// </summary>
    /// <param name="request">Dados de registro</param>
    /// <returns>Resultado com token e dados do usuario</returns>
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    
    /// <summary>
    /// Autentica um usuario existente
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Resultado com token e dados do usuario</returns>
    Task<AuthResult> LoginAsync(LoginRequest request);
    
    /// <summary>
    /// Verifica se um email ja esta em uso
    /// </summary>
    /// <param name="email">Email a verificar</param>
    /// <returns>True se email ja existe</returns>
    Task<bool> EmailExistsAsync(string email);
}
