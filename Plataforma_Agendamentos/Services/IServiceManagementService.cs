using Plataforma_Agendamentos.DTOs.Service;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Interface para gerenciamento de servicos oferecidos por prestadores
/// </summary>
public interface IServiceManagementService
{
    /// <summary>
    /// Lista todos os servicos de um prestador
    /// </summary>
    Task<IEnumerable<ServiceDto>> GetServicesByProviderAsync(Guid providerId);
    
    /// <summary>
    /// Obtem um servico especifico do prestador
    /// </summary>
    Task<ServiceDto?> GetServiceByIdAsync(Guid serviceId, Guid providerId);
    
    /// <summary>
    /// Cria um novo servico para o prestador
    /// </summary>
    Task<ServiceDto> CreateServiceAsync(Guid providerId, ServiceRequest request);
    
    /// <summary>
    /// Atualiza um servico existente
    /// </summary>
    Task<ServiceDto> UpdateServiceAsync(Guid serviceId, Guid providerId, ServiceRequest request);
    
    /// <summary>
    /// Remove um servico
    /// </summary>
    Task<bool> DeleteServiceAsync(Guid serviceId, Guid providerId);
}
