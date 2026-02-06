using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Extensions;
using Plataforma_Agendamentos.Services;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(IScheduleService scheduleService, ILogger<SchedulesController> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetSchedules()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem gerenciar horarios");

        var schedules = await _scheduleService.GetSchedulesByProviderAsync(userId.Value);
        return Ok(schedules);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem criar horarios");

        try
        {
            var schedule = await _scheduleService.CreateScheduleAsync(userId.Value, request);
            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, schedule);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao criar horario: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSchedule(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem visualizar horarios proprios");

        var schedule = await _scheduleService.GetScheduleByIdAsync(id, userId.Value);

        if (schedule == null)
            return NotFound();

        return Ok(schedule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] ScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem atualizar horarios");

        try
        {
            var schedule = await _scheduleService.UpdateScheduleAsync(id, userId.Value, request);
            return Ok(schedule);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao atualizar horario: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem excluir horarios");

        var deleted = await _scheduleService.DeleteScheduleAsync(id, userId.Value);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
