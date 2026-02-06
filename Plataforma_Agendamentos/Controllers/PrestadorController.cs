using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.Data;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/prestador")]
public class PrestadorController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrestadorController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var prestador = await _context.Users
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p.Branding)
            .Include(u => u.Services)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.PrestadorPerfil != null && 
                                    u.PrestadorPerfil.Slug == slug && 
                                    u.UserType == UserTypes.Prestador);

        if (prestador?.PrestadorPerfil == null)
            return NotFound("Prestador não encontrado.");

        var perfil = prestador.PrestadorPerfil;
        var branding = perfil.Branding;

        return Ok(new
        {
            perfil.Slug,
            perfil.DisplayName,
            perfil.TituloProfissional,
            perfil.Bio,
            perfil.Site,
            perfil.Telefone,
            perfil.Cidade,
            perfil.Estado,
            LogoUrl = branding?.LogoUrl,
            CoverImageUrl = branding?.CoverImageUrl,
            PrimaryColor = branding?.PrimaryColor,
            Services = prestador.Services.Select(s => new
            {
                s.Id,
                s.Nome,
                s.Description,
                s.Preco,
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
        var prestador = await _context.Users
            .Include(u => u.PrestadorPerfil)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.PrestadorPerfil != null && 
                                    u.PrestadorPerfil.Slug == slug && 
                                    u.UserType == UserTypes.Prestador);

        if (prestador == null)
            return NotFound("Prestador não encontrado.");

        var dayOfWeek = (int)date.DayOfWeek;
        var schedule = prestador.Schedules.FirstOrDefault(s => s.DayOfWeek == dayOfWeek);

        if (schedule == null)
            return Ok(new { AvailableTimes = new List<string>() });

        // Buscar agendamentos já confirmados para esta data
        var bookedTimes = await _context.Bookings
            .Where(b => b.Service.UserId == prestador.Id &&
                       b.Date.Date == date.Date &&
                       b.Status != BookingStatuses.Cancelado)
            .Select(b => b.Date.TimeOfDay)
            .ToListAsync();

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

        return Ok(new { AvailableTimes = availableTimes });
    }
}
