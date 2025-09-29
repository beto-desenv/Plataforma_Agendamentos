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
public class SchedulesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SchedulesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSchedules()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var schedules = await _context.Schedules
            .Where(s => s.ProviderId == userId)
            .Select(s => new
            {
                s.Id,
                s.DayOfWeek,
                s.StartTime,
                s.EndTime
            })
            .ToListAsync();

        return Ok(schedules);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        // Verificar se já existe horário para este dia da semana
        var existingSchedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ProviderId == userId && s.DayOfWeek == request.DayOfWeek);

        if (existingSchedule != null)
            return BadRequest("Já existe um horário cadastrado para este dia da semana.");

        if (request.StartTime >= request.EndTime)
            return BadRequest("Horário de início deve ser anterior ao horário de fim.");

        var schedule = new Schedule
        {
            ProviderId = userId.Value,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, new
        {
            schedule.Id,
            schedule.DayOfWeek,
            schedule.StartTime,
            schedule.EndTime
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSchedule(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var schedule = await _context.Schedules
            .Where(s => s.Id == id && s.ProviderId == userId)
            .Select(s => new
            {
                s.Id,
                s.DayOfWeek,
                s.StartTime,
                s.EndTime
            })
            .FirstOrDefaultAsync();

        if (schedule == null)
            return NotFound();

        return Ok(schedule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] ScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == userId);

        if (schedule == null)
            return NotFound();

        if (request.StartTime >= request.EndTime)
            return BadRequest("Horário de início deve ser anterior ao horário de fim.");

        // Verificar se já existe outro horário para este dia da semana
        var existingSchedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ProviderId == userId && s.DayOfWeek == request.DayOfWeek && s.Id != id);

        if (existingSchedule != null)
            return BadRequest("Já existe um horário cadastrado para este dia da semana.");

        schedule.DayOfWeek = request.DayOfWeek;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            schedule.Id,
            schedule.DayOfWeek,
            schedule.StartTime,
            schedule.EndTime
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == userId);

        if (schedule == null)
            return NotFound();

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}