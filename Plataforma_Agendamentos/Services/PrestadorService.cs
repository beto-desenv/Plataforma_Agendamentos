using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs.Prestador;
using Plataforma_Agendamentos.DTOs.Schedule;
using Plataforma_Agendamentos.DTOs.Service;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Service para dados publicos de prestadores
/// </summary>
public class PrestadorService : IPrestadorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PrestadorService> _logger;

    public PrestadorService(AppDbContext context, ILogger<PrestadorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PrestadorPublicDto?> GetBySlugAsync(string slug)
    {
        var prestador = await _context.Users
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p!.Branding)
            .Include(u => u.Services)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.PrestadorPerfil != null &&
                                    u.PrestadorPerfil.Slug == slug &&
                                    u.UserType == UserTypes.Prestador);

        if (prestador?.PrestadorPerfil == null)
        {
            _logger.LogWarning("Prestador com slug {Slug} nao encontrado", slug);
            return null;
        }

        var perfil = prestador.PrestadorPerfil;
        var branding = perfil.Branding;

        return new PrestadorPublicDto
        {
            Slug = perfil.Slug!,
            DisplayName = perfil.DisplayName,
            TituloProfissional = perfil.TituloProfissional,
            Bio = perfil.Bio,
            Site = perfil.Site,
            Telefone = perfil.Telefone,
            Cidade = perfil.Cidade,
            Estado = perfil.Estado,
            LogoUrl = branding?.LogoUrl,
            CoverImageUrl = branding?.CoverImageUrl,
            PrimaryColor = branding?.PrimaryColor,
            Services = prestador.Services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Description = s.Description,
                Preco = s.Preco,
                DurationMinutes = s.DurationMinutes
            }),
            Schedules = prestador.Schedules.Select(sc => new ScheduleDto
            {
                Id = sc.Id,
                DayOfWeek = sc.DayOfWeek,
                StartTime = sc.StartTime,
                EndTime = sc.EndTime
            })
        };
    }

    public async Task<AvailableTimeSlotsDto> GetAvailableTimeSlotsAsync(string slug, DateTime date)
    {
        var prestador = await _context.Users
            .Include(u => u.PrestadorPerfil)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.PrestadorPerfil != null &&
                                    u.PrestadorPerfil.Slug == slug &&
                                    u.UserType == UserTypes.Prestador);

        if (prestador == null)
        {
            _logger.LogWarning("Prestador com slug {Slug} nao encontrado para calculo de horarios", slug);
            return new AvailableTimeSlotsDto();
        }

        var dayOfWeek = (int)date.DayOfWeek;
        var schedule = prestador.Schedules.FirstOrDefault(s => s.DayOfWeek == dayOfWeek);

        if (schedule == null)
        {
            _logger.LogInformation("Prestador {Slug} nao tem agenda para dia {DayOfWeek}", slug, dayOfWeek);
            return new AvailableTimeSlotsDto();
        }

        // Buscar agendamentos ja confirmados para esta data
        var bookedTimes = await _context.Bookings
            .Where(b => b.Service.UserId == prestador.Id &&
                       b.Date.Date == date.Date &&
                       b.Status != BookingStatuses.Cancelado)
            .Select(b => b.Date.TimeOfDay)
            .ToListAsync();

        // Gerar horarios disponiveis de 30 em 30 minutos
        var availableTimes = GenerateTimeSlots(schedule.StartTime, schedule.EndTime, bookedTimes);

        _logger.LogInformation("Calculados {Count} horarios disponiveis para {Slug} em {Date}", 
            availableTimes.Count(), slug, date.Date);

        return new AvailableTimeSlotsDto
        {
            AvailableTimes = availableTimes
        };
    }

    /// <summary>
    /// Gera slots de horario de 30 em 30 minutos
    /// </summary>
    private static IEnumerable<string> GenerateTimeSlots(TimeSpan startTime, TimeSpan endTime, List<TimeSpan> bookedTimes)
    {
        var availableTimes = new List<string>();
        var currentTime = startTime;
        var slotDuration = TimeSpan.FromMinutes(30);

        while (currentTime.Add(slotDuration) <= endTime)
        {
            if (!bookedTimes.Contains(currentTime))
            {
                availableTimes.Add(currentTime.ToString(@"hh\:mm"));
            }
            currentTime = currentTime.Add(slotDuration);
        }

        return availableTimes;
    }
}
