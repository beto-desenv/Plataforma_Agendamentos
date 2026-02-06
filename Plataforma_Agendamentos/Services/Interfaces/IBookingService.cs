using Plataforma_Agendamentos.DTOs.Booking;

namespace Plataforma_Agendamentos.Services.Interfaces;

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
