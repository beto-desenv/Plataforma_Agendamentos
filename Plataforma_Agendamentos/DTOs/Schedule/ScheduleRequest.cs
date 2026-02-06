using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs.Schedule;

public class ScheduleRequest
{
    [Required]
    [Range(0, 6)]
    public int DayOfWeek { get; set; }
    
    [Required]
    public TimeSpan StartTime { get; set; }
    
    [Required]
    public TimeSpan EndTime { get; set; }
}
