using Plataforma_Agendamentos.DTOs.Booking;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Interface para servico de agendamentos
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Lista agendamentos do usuario (cliente ve seus agendamentos, prestador ve agendamentos de seus servicos)
    /// </summary>
    Task<IEnumerable<BookingDto>> GetBookingsByUserAsync(Guid userId, string userType);
    
    /// <summary>
    /// Obtem um agendamento especifico
    /// </summary>
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, Guid userId, string userType);
    
    /// <summary>
    /// Cria um novo agendamento com validacoes de negocio
    /// </summary>
    Task<BookingDto> CreateBookingAsync(Guid clientId, BookingRequest request);
    
    /// <summary>
    /// Atualiza status do agendamento (apenas prestador)
    /// </summary>
    Task<bool> UpdateBookingStatusAsync(Guid bookingId, Guid prestadorId, string newStatus);
    
    /// <summary>
    /// Verifica se um horario esta disponivel
    /// </summary>
    Task<bool> IsTimeSlotAvailableAsync(Guid serviceId, DateTime dateTime);
}

/// <summary>
/// DTO para retorno de agendamento
/// </summary>
public class BookingDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public ClientInfoDto Client { get; set; } = null!;
    public ServiceInfoDto Service { get; set; } = null!;
}

public class ClientInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ServiceInfoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DurationMinutes { get; set; }
    public ProviderInfoDto Provider { get; set; } = null!;
}

public class ProviderInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
