using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Extensions;
using Plataforma_Agendamentos.Models;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var userType = GetCurrentUserType();

        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
            .ThenInclude(s => s.User);

        if (userType == UserTypes.Cliente)
        {
            query = query.Where(b => b.ClientId == userId);
        }
        else if (userType == UserTypes.Prestador)
        {
            query = query.Where(b => b.Service.UserId == userId);
        }

        var bookings = await query
            .Select(b => new
            {
                b.Id,
                b.Date,
                b.Status,
                Client = new
                {
                    b.Client.Name,
                    b.Client.Email
                },
                Service = new
                {
                    b.Service.Nome,
                    b.Service.Preco,
                    b.Service.DurationMinutes,
                    Provider = new
                    {
                        b.Service.User.Name,
                        b.Service.User.DisplayName
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

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var userType = User.GetUserType();
        if (userType != UserTypes.Cliente)
            return Forbid("Apenas clientes podem fazer agendamentos.");

        if (request.Date <= DateTime.Now)
            return BadRequest("A data do agendamento deve ser futura.");

        var service = await _context.Services
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId);

        if (service == null)
            return BadRequest("Serviço não encontrado.");

        // Verificar se a data/hora está disponível
        var dayOfWeek = (int)request.Date.DayOfWeek;
        var timeOfDay = request.Date.TimeOfDay;

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ProviderId == service.UserId && 
                                    s.DayOfWeek == dayOfWeek &&
                                    s.StartTime <= timeOfDay &&
                                    s.EndTime > timeOfDay);

        if (schedule == null)
            return BadRequest("Horário não disponível para este prestador.");

        // Verificar se já existe agendamento para este horário
        var existingBooking = await _context.Bookings
            .AnyAsync(b => b.ServiceId == request.ServiceId && 
                          b.Date == request.Date && 
                          b.Status != BookingStatuses.Cancelado);

        if (existingBooking)
            return BadRequest("Este horário já está ocupado.");

        var booking = new Booking
        {
            ClientId = userId.Value,
            ServiceId = request.ServiceId,
            Date = request.Date,
            Status = BookingStatuses.Pendente
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Recarregar com dados relacionados
        booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
            .ThenInclude(s => s.User)
            .FirstAsync(b => b.Id == booking.Id);

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
                booking.Service.Nome,
                booking.Service.Preco,
                booking.Service.DurationMinutes,
                Provider = new
                {
                    booking.Service.User.Name,
                    booking.Service.User.DisplayName
                }
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var userType = User.GetUserType();
        if (userType == null)
            return Unauthorized();

        var booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Service)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound();

        // Verificar se o usuário tem acesso a este agendamento
        bool hasAccess = userType == UserTypes.Cliente ? booking.ClientId == userId : booking.Service.UserId == userId;
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
                booking.Service.Nome,
                booking.Service.Preco,
                booking.Service.DurationMinutes,
                Provider = new
                {
                    booking.Service.User.Name,
                    booking.Service.User.DisplayName
                }
            }
        });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateBookingStatus(Guid id, [FromBody] string status)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var userType = User.GetUserType();
        if (userType != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem alterar o status do agendamento.");

        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("Status deve ser 'confirmado' ou 'cancelado'.");

        var normalizedStatus = status.Trim().ToLowerInvariant();
        if (normalizedStatus != BookingStatuses.Confirmado && normalizedStatus != BookingStatuses.Cancelado)
            return BadRequest("Status deve ser 'confirmado' ou 'cancelado'.");

        var booking = await _context.Bookings
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound();

        if (booking.Service.UserId != userId)
            return Forbid();

        booking.Status = normalizedStatus;
        await _context.SaveChangesAsync();

        return Ok(new { booking.Id, booking.Status });
    }

    private string? GetCurrentUserType()
    {
        return User.GetUserType();
    }
}
