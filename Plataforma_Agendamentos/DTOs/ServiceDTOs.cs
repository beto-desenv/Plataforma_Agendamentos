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

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Preco { get; set; }
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

// Adicionar DTO de resposta para Schedule
public class ScheduleDto
{
    public Guid Id { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

/// <summary>
/// DTO para horarios disponiveis (movido de PrestadorDTOs.cs)
/// </summary>
public class AvailableTimeSlotsDto
{
    public IEnumerable<string> AvailableTimes { get; set; } = new List<string>();
}

public class BookingRequest
{
    [Required]
    public Guid ServiceId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public string? Notes { get; set; }
}

// DTOs de Profile movidos de ProfileController
public class UpdateClienteProfileRequest
{
    public string? Name { get; set; }
    public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }
    public string? CPF { get; set; }
    public string? PreferenciasNotificacao { get; set; }
}

public class UpdatePrestadorProfileRequest
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Slug { get; set; }
    public string? TituloProfissional { get; set; }
    public string? Bio { get; set; }
    public string? CPF { get; set; }
    public string? CNPJ { get; set; }
    public int? AnosExperiencia { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }
    public string? Site { get; set; }
    public int? RaioAtendimento { get; set; }
    public bool? AceitaAgendamentoImediato { get; set; }
    public int? HorasAntecedenciaMinima { get; set; }
    public string? HorarioInicioSemana { get; set; }
    public string? HorarioFimSemana { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
}