using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs;

public class ServiceRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }
}

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

public class BookingRequest
{
    [Required]
    public Guid ServiceId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
}