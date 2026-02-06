using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.DTOs.Booking;
using Plataforma_Agendamentos.Extensions;
using Plataforma_Agendamentos.Services;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var userType = User.GetUserType();
        if (userType == null)
            return Unauthorized();

        var bookings = await _bookingService.GetBookingsByUserAsync(userId.Value, userType);
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

        try
        {
            var booking = await _bookingService.CreateBookingAsync(userId.Value, request);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao criar agendamento: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
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

        var booking = await _bookingService.GetBookingByIdAsync(id, userId.Value, userType);
        
        if (booking == null)
            return NotFound();

        return Ok(booking);
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
            return BadRequest("Status nao pode estar vazio");

        try
        {
            var success = await _bookingService.UpdateBookingStatusAsync(id, userId.Value, status);
            
            if (!success)
                return NotFound();

            return Ok(new { id, status = status.Trim().ToLowerInvariant() });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao atualizar status: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
