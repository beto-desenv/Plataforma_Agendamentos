using Plataforma_Agendamentos.DTOs.Profile;

namespace Plataforma_Agendamentos.Services.Interfaces;

/// <summary>
/// Interface para servico de gerenciamento de perfis
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Obtem perfil completo do usuario (cliente ou prestador)
    /// </summary>
    Task<object?> GetProfileAsync(Guid userId);
    
    /// <summary>
    /// Atualiza perfil de cliente
    /// </summary>
    Task<object> UpdateClienteProfileAsync(Guid userId, UpdateClienteProfileRequest request);
    
    /// <summary>
    /// Atualiza perfil de prestador
    /// </summary>
    Task<object> UpdatePrestadorProfileAsync(Guid userId, UpdatePrestadorProfileRequest request);
    
    /// <summary>
    /// Obtem perfil publico de prestador por slug
    /// </summary>
    Task<object?> GetPrestadorBySlugAsync(string slug);
    
    /// <summary>
    /// Verifica se slug ja esta em uso
    /// </summary>
    Task<bool> SlugExistsAsync(string slug, Guid? excludeUserId = null);
}
