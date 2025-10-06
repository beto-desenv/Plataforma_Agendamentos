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
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(AppDbContext context, ILogger<ServicesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se o usuário é prestador
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || !user.IsPrestador())
            return Forbid("Apenas prestadores podem gerenciar serviços.");

        var services = await _context.Services
            .Where(s => s.ProviderId == userId)
            .Select(s => new
            {
                s.Id,
                s.Title,
                s.Description,
                s.Price,
                s.DurationMinutes
            })
            .ToListAsync();

        _logger.LogInformation("Listados {Count} serviços para prestador {UserId}", services.Count, userId);

        return Ok(services);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] ServiceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se o usuário é prestador
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || !user.IsPrestador())
            return Forbid("Apenas prestadores podem criar serviços.");

        var service = new Service
        {
            ProviderId = userId.Value,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Serviço criado: {ServiceId} - {Title} para prestador {UserId}", 
            service.Id, service.Title, userId);

        return CreatedAtAction(nameof(GetService), new { id = service.Id }, new
        {
            service.Id,
            service.Title,
            service.Description,
            service.Price,
            service.DurationMinutes
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se o usuário é prestador
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || !user.IsPrestador())
            return Forbid("Apenas prestadores podem acessar serviços.");

        var service = await _context.Services
            .Where(s => s.Id == id && s.ProviderId == userId)
            .Select(s => new
            {
                s.Id,
                s.Title,
                s.Description,
                s.Price,
                s.DurationMinutes
            })
            .FirstOrDefaultAsync();

        if (service == null)
            return NotFound();

        return Ok(service);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] ServiceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se o usuário é prestador
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || !user.IsPrestador())
            return Forbid("Apenas prestadores podem atualizar serviços.");

        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == userId);

        if (service == null)
            return NotFound();

        service.Title = request.Title;
        service.Description = request.Description;
        service.Price = request.Price;
        service.DurationMinutes = request.DurationMinutes;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Serviço atualizado: {ServiceId} - {Title} pelo prestador {UserId}", 
            service.Id, service.Title, userId);

        return Ok(new
        {
            service.Id,
            service.Title,
            service.Description,
            service.Price,
            service.DurationMinutes
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se o usuário é prestador
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || !user.IsPrestador())
            return Forbid("Apenas prestadores podem excluir serviços.");

        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == userId);

        if (service == null)
            return NotFound();

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Serviço removido: {ServiceId} - {Title} pelo prestador {UserId}", 
            service.Id, service.Title, userId);

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}