using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Models;
using System.Security.Claims;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(AppDbContext context, ILogger<BookingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Buscar o usuário para verificar seus roles
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized();

        var roles = user.GetRoles();
        _logger.LogInformation("Usuário {UserId} possui roles: {Roles}", userId, string.Join(", ", roles));

        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
            .ThenInclude(s => s.Provider);

        // Se é cliente, mostrar agendamentos que fez
        // Se é prestador, mostrar agendamentos dos seus serviços
        // Se é ambos, mostrar todos (pode ser refinado depois)
        if (user.IsCliente() && !user.IsPrestador())
        {
            // Apenas cliente
            query = query.Where(b => b.ClientId == userId);
        }
        else if (user.IsPrestador() && !user.IsCliente())
        {
            // Apenas prestador
            query = query.Where(b => b.Service.ProviderId == userId);
        }
        else if (user.IsCliente() && user.IsPrestador())
        {
            // Cliente e prestador - mostrar ambos
            query = query.Where(b => b.ClientId == userId || b.Service.ProviderId == userId);
        }

        var bookings = await query
            .Select(b => new
            {
                b.Id,
                b.Date,
                b.Status,
                Type = b.ClientId == userId ? "cliente" : "prestador", // Indica se é agendamento feito ou recebido
                Client = new
                {
                    b.Client.Name,
                    b.Client.Email
                },
                Service = new
                {
                    b.Service.Title,
                    b.Service.Price,
                    b.Service.DurationMinutes,
                    Provider = new
                    {
                        b.Service.Provider.Name,
                        b.Service.Provider.DisplayName
                    }
                }
            })
            .ToListAsync();

        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se o usuário tem role de cliente
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || !user.IsCliente())
            return Forbid("Apenas clientes podem fazer agendamentos.");

        var service = await _context.Services
            .Include(s => s.Provider)
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId);

        if (service == null)
            return BadRequest("Serviço não encontrado.");

        // Verificar se a data/hora está disponível
        var dayOfWeek = (int)request.Date.DayOfWeek;
        var timeOfDay = request.Date.TimeOfDay;

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ProviderId == service.ProviderId && 
                                    s.DayOfWeek == dayOfWeek &&
                                    s.StartTime <= timeOfDay &&
                                    s.EndTime > timeOfDay);

        if (schedule == null)
            return BadRequest("Horário não disponível para este prestador.");

        // Verificar se já existe agendamento para este horário
        var existingBooking = await _context.Bookings
            .AnyAsync(b => b.ServiceId == request.ServiceId && 
                          b.Date == request.Date && 
                          b.Status != "cancelado");

        if (existingBooking)
            return BadRequest("Este horário já está ocupado.");

        var booking = new Booking
        {
            ClientId = userId.Value,
            ServiceId = request.ServiceId,
            Date = request.Date,
            Status = "pendente"
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Recarregar com dados relacionados
        booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
            .ThenInclude(s => s.Provider)
            .FirstAsync(b => b.Id == booking.Id);

        _logger.LogInformation("Agendamento criado: {BookingId} para cliente {ClientId} no serviço {ServiceId}", 
            booking.Id, userId, request.ServiceId);

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, new
        {
            booking.Id,
            booking.Date,
            booking.Status,
            Client = new
            {
                booking.Client.Name,
                booking.Client.Email
            },
            Service = new
            {
                booking.Service.Title,
                booking.Service.Price,
                booking.Service.DurationMinutes,
                Provider = new
                {
                    booking.Service.Provider.Name,
                    booking.Service.Provider.DisplayName
                }
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
            .ThenInclude(s => s.Provider)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound();

        // Verificar se o usuário tem acesso a este agendamento
        bool hasAccess = booking.ClientId == userId || booking.Service.ProviderId == userId;
        if (!hasAccess)
            return Forbid();

        return Ok(new
        {
            booking.Id,
            booking.Date,
            booking.Status,
            Client = new
            {
                booking.Client.Name,
                booking.Client.Email
            },
            Service = new
            {
                booking.Service.Title,
                booking.Service.Price,
                booking.Service.DurationMinutes,
                Provider = new
                {
                    booking.Service.Provider.Name,
                    booking.Service.Provider.DisplayName
                }
            }
        });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateBookingStatus(Guid id, [FromBody] string status)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se o usuário tem role de prestador
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || !user.IsPrestador())
            return Forbid("Apenas prestadores podem alterar o status do agendamento.");

        if (!new[] { "confirmado", "cancelado" }.Contains(status))
            return BadRequest("Status deve ser 'confirmado' ou 'cancelado'.");

        var booking = await _context.Bookings
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound();

        if (booking.Service.ProviderId != userId)
            return Forbid();

        booking.Status = status;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Status do agendamento {BookingId} alterado para {Status} pelo prestador {ProviderId}", 
            id, status, userId);

        return Ok(new { booking.Id, booking.Status });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}