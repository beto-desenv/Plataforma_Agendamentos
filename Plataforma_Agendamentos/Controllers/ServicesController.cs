using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.DTOs.Service;
using Plataforma_Agendamentos.Extensions;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceManagementService _serviceManagementService;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(IServiceManagementService serviceManagementService, ILogger<ServicesController> logger)
    {
        _serviceManagementService = serviceManagementService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem gerenciar servicos");

        var services = await _serviceManagementService.GetServicesByProviderAsync(userId.Value);
        return Ok(services);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] ServiceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem criar servicos");

        var service = await _serviceManagementService.CreateServiceAsync(userId.Value, request);
        return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem visualizar servicos proprios");

        var service = await _serviceManagementService.GetServiceByIdAsync(id, userId.Value);

        if (service == null)
            return NotFound();

        return Ok(service);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] ServiceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem atualizar servicos");

        try
        {
            var service = await _serviceManagementService.UpdateServiceAsync(id, userId.Value, request);
            return Ok(service);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao atualizar servico: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (User.GetUserType() != UserTypes.Prestador)
            return Forbid("Apenas prestadores podem excluir servicos");

        var deleted = await _serviceManagementService.DeleteServiceAsync(id, userId.Value);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
