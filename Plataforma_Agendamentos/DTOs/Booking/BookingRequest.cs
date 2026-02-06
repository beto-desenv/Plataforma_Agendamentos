using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs.Booking;

public class BookingRequest
{
    [Required]
    public Guid ServiceId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public string? Notes { get; set; }
}
