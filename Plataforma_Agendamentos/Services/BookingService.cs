using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Models;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Service para gerenciar regras de negocio de agendamentos
/// </summary>
public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<BookingService> _logger;

    public BookingService(AppDbContext context, ILogger<BookingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByUserAsync(Guid userId, string userType)
    {
        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
                .ThenInclude(s => s.User)
                    .ThenInclude(u => u.PrestadorPerfil);

        // Filtrar baseado no tipo de usuario
        if (userType == UserTypes.Cliente)
        {
            query = query.Where(b => b.ClientId == userId);
        }
        else if (userType == UserTypes.Prestador)
        {
            query = query.Where(b => b.Service.UserId == userId);
        }

        var bookings = await query.ToListAsync();

        return bookings.Select(MapToDto);
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, Guid userId, string userType)
    {
        var booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
                .ThenInclude(s => s.User)
                    .ThenInclude(u => u.PrestadorPerfil)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return null;

        // Verificar acesso
        bool hasAccess = userType == UserTypes.Cliente 
            ? booking.ClientId == userId 
            : booking.Service.UserId == userId;

        if (!hasAccess)
        {
            _logger.LogWarning("Usuario {UserId} tentou acessar agendamento {BookingId} sem permissao", userId, bookingId);
            return null;
        }

        return MapToDto(booking);
    }

    public async Task<BookingDto> CreateBookingAsync(Guid clientId, BookingRequest request)
    {
        // Validacao: Data futura
        if (request.Date <= DateTime.Now)
        {
            throw new InvalidOperationException("A data do agendamento deve ser futura");
        }

        // Buscar servico
        var service = await _context.Services
            .Include(s => s.User)
                .ThenInclude(u => u.PrestadorPerfil)
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId);

        if (service == null)
        {
            throw new InvalidOperationException("Servico nao encontrado");
        }

        // Validacao: Verificar disponibilidade de horario
        var dayOfWeek = (int)request.Date.DayOfWeek;
        var timeOfDay = request.Date.TimeOfDay;

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ProviderId == service.UserId && 
                                    s.DayOfWeek == dayOfWeek &&
                                    s.StartTime <= timeOfDay &&
                                    s.EndTime > timeOfDay);

        if (schedule == null)
        {
            throw new InvalidOperationException("Horario nao disponivel para este prestador");
        }

        // Validacao: Verificar conflitos
        var hasConflict = await _context.Bookings
            .AnyAsync(b => b.ServiceId == request.ServiceId && 
                          b.Date == request.Date && 
                          b.Status != BookingStatuses.Cancelado);

        if (hasConflict)
        {
            throw new InvalidOperationException("Este horario ja esta ocupado");
        }

        // Criar agendamento
        var booking = new Booking
        {
            ClientId = clientId,
            ServiceId = request.ServiceId,
            Date = request.Date,
            Status = BookingStatuses.Pendente
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Agendamento criado: {BookingId} para cliente {ClientId}", booking.Id, clientId);

        // Recarregar com dados relacionados
        booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
                .ThenInclude(s => s.User)
                    .ThenInclude(u => u.PrestadorPerfil)
            .FirstAsync(b => b.Id == booking.Id);

        return MapToDto(booking);
    }

    public async Task<bool> UpdateBookingStatusAsync(Guid bookingId, Guid prestadorId, string newStatus)
    {
        // Validacao de status
        var normalizedStatus = newStatus.Trim().ToLowerInvariant();
        if (normalizedStatus != BookingStatuses.Confirmado && normalizedStatus != BookingStatuses.Cancelado)
        {
            throw new InvalidOperationException("Status deve ser 'confirmado' ou 'cancelado'");
        }

        var booking = await _context.Bookings
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            _logger.LogWarning("Agendamento {BookingId} nao encontrado", bookingId);
            return false;
        }

        // Verificar se o prestador e dono do servico
        if (booking.Service.UserId != prestadorId)
        {
            _logger.LogWarning("Prestador {PrestadorId} tentou alterar agendamento {BookingId} sem permissao", prestadorId, bookingId);
            return false;
        }

        booking.Status = normalizedStatus;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Status do agendamento {BookingId} alterado para {Status}", bookingId, normalizedStatus);

        return true;
    }

    public async Task<bool> IsTimeSlotAvailableAsync(Guid serviceId, DateTime dateTime)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null)
            return false;

        var dayOfWeek = (int)dateTime.DayOfWeek;
        var timeOfDay = dateTime.TimeOfDay;

        // Verificar se existe agenda para este dia/horario
        var hasSchedule = await _context.Schedules
            .AnyAsync(s => s.ProviderId == service.UserId && 
                          s.DayOfWeek == dayOfWeek &&
                          s.StartTime <= timeOfDay &&
                          s.EndTime > timeOfDay);

        if (!hasSchedule)
            return false;

        // Verificar se nao ha conflito
        var hasConflict = await _context.Bookings
            .AnyAsync(b => b.ServiceId == serviceId && 
                          b.Date == dateTime && 
                          b.Status != BookingStatuses.Cancelado);

        return !hasConflict;
    }

    // Helper para mapear entidade para DTO
    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            Date = booking.Date,
            Status = booking.Status,
            Client = new ClientInfoDto
            {
                Name = booking.Client.Name,
                Email = booking.Client.Email
            },
            Service = new ServiceInfoDto
            {
                Nome = booking.Service.Nome,
                Preco = booking.Service.Preco,
                DurationMinutes = booking.Service.DurationMinutes,
                Provider = new ProviderInfoDto
                {
                    Name = booking.Service.User.Name,
                    DisplayName = booking.Service.User.PrestadorPerfil?.DisplayName ?? booking.Service.User.Name
                }
            }
        };
    }
}
