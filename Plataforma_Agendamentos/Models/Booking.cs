using System.ComponentModel.DataAnnotations;
using Plataforma_Agendamentos.Constants;

namespace Plataforma_Agendamentos.Models;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ClientId { get; set; }
    
    [Required]
    public Guid ServiceId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = BookingStatuses.Pendente; // pendente, confirmado, cancelado

    public User Client { get; set; } = null!;
    public Service Service { get; set; } = null!;
}
