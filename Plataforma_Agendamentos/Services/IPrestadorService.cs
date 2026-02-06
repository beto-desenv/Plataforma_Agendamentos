using Plataforma_Agendamentos.DTOs.Prestador;
using Plataforma_Agendamentos.DTOs.Schedule;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Interface para servicos publicos de prestador
/// </summary>
public interface IPrestadorService
{
    /// <summary>
    /// Busca prestador por slug (perfil publico)
    /// </summary>
    Task<PrestadorPublicDto?> GetBySlugAsync(string slug);
    
    /// <summary>
    /// Calcula horarios disponiveis de um prestador para uma data especifica
    /// </summary>
    Task<AvailableTimeSlotsDto> GetAvailableTimeSlotsAsync(string slug, DateTime date);
}
