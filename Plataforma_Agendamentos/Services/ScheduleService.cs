using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Models;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Service para gerenciar horarios de atendimento de prestadores
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(AppDbContext context, ILogger<ScheduleService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ScheduleDto>> GetSchedulesByProviderAsync(Guid providerId)
    {
        var schedules = await _context.Schedules
            .Where(s => s.ProviderId == providerId)
            .ToListAsync();

        return schedules.Select(MapToDto);
    }

    public async Task<ScheduleDto?> GetScheduleByIdAsync(Guid scheduleId, Guid providerId)
    {
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.ProviderId == providerId);

        if (schedule == null)
        {
            _logger.LogWarning("Horario {ScheduleId} nao encontrado para prestador {ProviderId}", scheduleId, providerId);
            return null;
        }

        return MapToDto(schedule);
    }

    public async Task<ScheduleDto> CreateScheduleAsync(Guid providerId, ScheduleRequest request)
    {
        // Validacao: horario de inicio antes do fim
        if (request.StartTime >= request.EndTime)
        {
            throw new InvalidOperationException("Horario de inicio deve ser anterior ao horario de fim");
        }

        // Validacao: verificar se ja existe horario para este dia
        var existingSchedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ProviderId == providerId && s.DayOfWeek == request.DayOfWeek);

        if (existingSchedule != null)
        {
            throw new InvalidOperationException("Ja existe um horario cadastrado para este dia da semana");
        }

        var schedule = new Schedule
        {
            ProviderId = providerId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Horario criado: {ScheduleId} para prestador {ProviderId}, Dia: {DayOfWeek}", 
            schedule.Id, providerId, request.DayOfWeek);

        return MapToDto(schedule);
    }

    public async Task<ScheduleDto> UpdateScheduleAsync(Guid scheduleId, Guid providerId, ScheduleRequest request)
    {
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.ProviderId == providerId);

        if (schedule == null)
        {
            throw new InvalidOperationException("Horario nao encontrado ou nao pertence ao prestador");
        }

        // Validacao: horario de inicio antes do fim
        if (request.StartTime >= request.EndTime)
        {
            throw new InvalidOperationException("Horario de inicio deve ser anterior ao horario de fim");
        }

        // Validacao: verificar se ja existe outro horario para este dia
        var existingSchedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ProviderId == providerId && 
                                    s.DayOfWeek == request.DayOfWeek && 
                                    s.Id != scheduleId);

        if (existingSchedule != null)
        {
            throw new InvalidOperationException("Ja existe um horario cadastrado para este dia da semana");
        }

        schedule.DayOfWeek = request.DayOfWeek;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Horario atualizado: {ScheduleId}", scheduleId);

        return MapToDto(schedule);
    }

    public async Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid providerId)
    {
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.ProviderId == providerId);

        if (schedule == null)
        {
            _logger.LogWarning("Tentativa de deletar horario inexistente: {ScheduleId}", scheduleId);
            return false;
        }

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Horario deletado: {ScheduleId}", scheduleId);

        return true;
    }

    private static ScheduleDto MapToDto(Schedule schedule)
    {
        return new ScheduleDto
        {
            Id = schedule.Id,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };
    }
}
