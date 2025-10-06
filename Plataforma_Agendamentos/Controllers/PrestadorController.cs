using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Data;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/prestador")]
public class PrestadorController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<PrestadorController> _logger;

    public PrestadorController(AppDbContext context, ILogger<PrestadorController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        _logger.LogInformation("Buscando prestador com slug: {Slug}", slug);

        var prestador = await _context.Users
            .Include(u => u.Services)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.Slug == slug);

        if (prestador == null)
        {
            _logger.LogWarning("Prestador não encontrado com slug: {Slug}", slug);
            return NotFound("Prestador não encontrado.");
        }

        // Verificar se o usuário tem o role de prestador
        if (!prestador.IsPrestador())
        {
            _logger.LogWarning("Usuário {Slug} não possui role de prestador. Roles: {Roles}", 
                slug, string.Join(", ", prestador.GetRoles()));
            return NotFound("Prestador não encontrado.");
        }

        _logger.LogInformation("Prestador encontrado: {Slug} com {ServicesCount} serviços e {SchedulesCount} horários", 
            slug, prestador.Services.Count, prestador.Schedules.Count);

        return Ok(new
        {
            prestador.Slug,
            prestador.DisplayName,
            prestador.LogoUrl,
            prestador.CoverImageUrl,
            prestador.PrimaryColor,
            prestador.Bio,
            Services = prestador.Services.Select(s => new
            {
                s.Id,
                s.Title,
                s.Description,
                s.Price,
                s.DurationMinutes
            }),
            Schedules = prestador.Schedules.Select(sc => new
            {
                sc.Id,
                sc.DayOfWeek,
                sc.StartTime,
                sc.EndTime
            })
        });
    }

    [HttpGet("{slug}/available-times")]
    public async Task<IActionResult> GetAvailableTimes(string slug, [FromQuery] DateTime date)
    {
        _logger.LogInformation("Buscando horários disponíveis para {Slug} na data {Date}", slug, date.ToString("yyyy-MM-dd"));

        var prestador = await _context.Users
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.Slug == slug);

        if (prestador == null || !prestador.IsPrestador())
        {
            _logger.LogWarning("Prestador não encontrado ou não possui role de prestador: {Slug}", slug);
            return NotFound("Prestador não encontrado.");
        }

        var dayOfWeek = (int)date.DayOfWeek;
        var schedule = prestador.Schedules.FirstOrDefault(s => s.DayOfWeek == dayOfWeek);

        if (schedule == null)
        {
            _logger.LogInformation("Nenhum horário configurado para {Slug} no dia da semana {DayOfWeek}", slug, dayOfWeek);
            return Ok(new { AvailableTimes = new List<string>() });
        }

        // Buscar agendamentos já confirmados para esta data
        var bookedTimes = await _context.Bookings
            .Where(b => b.Service.ProviderId == prestador.Id &&
                       b.Date.Date == date.Date &&
                       b.Status != "cancelado")
            .Select(b => b.Date.TimeOfDay)
            .ToListAsync();

        _logger.LogInformation("Encontrados {BookedCount} horários já reservados para {Slug} em {Date}", 
            bookedTimes.Count, slug, date.ToString("yyyy-MM-dd"));

        // Gerar horários disponíveis de 30 em 30 minutos
        var availableTimes = new List<string>();
        var currentTime = schedule.StartTime;
        var endTime = schedule.EndTime;

        while (currentTime.Add(TimeSpan.FromMinutes(30)) <= endTime)
        {
            if (!bookedTimes.Contains(currentTime))
            {
                availableTimes.Add(currentTime.ToString(@"hh\:mm"));
            }
            currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
        }

        _logger.LogInformation("Gerados {AvailableCount} horários disponíveis para {Slug} em {Date}", 
            availableTimes.Count, slug, date.ToString("yyyy-MM-dd"));

        return Ok(new { AvailableTimes = availableTimes });
    }
}