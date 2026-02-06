using Plataforma_Agendamentos.DTOs.Schedule;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Interface para gerenciamento de horarios de atendimento
/// </summary>
public interface IScheduleService
{
    /// <summary>
    /// Lista todos os horarios de um prestador
    /// </summary>
    Task<IEnumerable<ScheduleDto>> GetSchedulesByProviderAsync(Guid providerId);
    
    /// <summary>
    /// Obtem um horario especifico do prestador
    /// </summary>
    Task<ScheduleDto?> GetScheduleByIdAsync(Guid scheduleId, Guid providerId);
    
    /// <summary>
    /// Cria um novo horario de atendimento
    /// </summary>
    Task<ScheduleDto> CreateScheduleAsync(Guid providerId, ScheduleRequest request);
    
    /// <summary>
    /// Atualiza um horario existente
    /// </summary>
    Task<ScheduleDto> UpdateScheduleAsync(Guid scheduleId, Guid providerId, ScheduleRequest request);
    
    /// <summary>
    /// Remove um horario
    /// </summary>
    Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid providerId);
}
