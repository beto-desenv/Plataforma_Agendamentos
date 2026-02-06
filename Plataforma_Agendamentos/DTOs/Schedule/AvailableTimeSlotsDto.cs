namespace Plataforma_Agendamentos.DTOs.Schedule;

/// <summary>
/// DTO para horarios disponiveis
/// </summary>
public class AvailableTimeSlotsDto
{
    public IEnumerable<string> AvailableTimes { get; set; } = new List<string>();
}
