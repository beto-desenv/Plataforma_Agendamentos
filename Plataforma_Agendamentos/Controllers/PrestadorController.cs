using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/prestador")]
public class PrestadorController : ControllerBase
{
    private readonly IPrestadorService _prestadorService;
    private readonly ILogger<PrestadorController> _logger;

    public PrestadorController(IPrestadorService prestadorService, ILogger<PrestadorController> logger)
    {
        _prestadorService = prestadorService;
        _logger = logger;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var prestador = await _prestadorService.GetBySlugAsync(slug);

        if (prestador == null)
            return NotFound("Prestador nao encontrado");

        return Ok(prestador);
    }

    [HttpGet("{slug}/available-times")]
    public async Task<IActionResult> GetAvailableTimes(string slug, [FromQuery] DateTime date)
    {
        var availableTimes = await _prestadorService.GetAvailableTimeSlotsAsync(slug, date);
        return Ok(availableTimes);
    }
}
