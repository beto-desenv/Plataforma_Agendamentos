using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

public class Schedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProviderId { get; set; }
    
    [Required]
    public int DayOfWeek { get; set; } // 0 = Domingo, 6 = Sábado
    
    [Required]
    public TimeSpan StartTime { get; set; }
    
    [Required]
    public TimeSpan EndTime { get; set; }

    public User Provider { get; set; } = null!;
}