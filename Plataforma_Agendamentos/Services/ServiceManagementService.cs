using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs.Service;
using Plataforma_Agendamentos.Models;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Service para gerenciar CRUD de servicos
/// </summary>
public class ServiceManagementService : IServiceManagementService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ServiceManagementService> _logger;

    public ServiceManagementService(AppDbContext context, ILogger<ServiceManagementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesByProviderAsync(Guid providerId)
    {
        var services = await _context.Services
            .Where(s => s.UserId == providerId)
            .ToListAsync();

        return services.Select(MapToDto);
    }

    public async Task<ServiceDto?> GetServiceByIdAsync(Guid serviceId, Guid providerId)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.UserId == providerId);

        if (service == null)
        {
            _logger.LogWarning("Servico {ServiceId} nao encontrado para prestador {ProviderId}", serviceId, providerId);
            return null;
        }

        return MapToDto(service);
    }

    public async Task<ServiceDto> CreateServiceAsync(Guid providerId, ServiceRequest request)
    {
        var service = new Service
        {
            UserId = providerId,
            Nome = request.Title,
            Description = request.Description,
            Preco = request.Price,
            DurationMinutes = request.DurationMinutes
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Servico criado: {ServiceId} para prestador {ProviderId}", service.Id, providerId);

        return MapToDto(service);
    }

    public async Task<ServiceDto> UpdateServiceAsync(Guid serviceId, Guid providerId, ServiceRequest request)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.UserId == providerId);

        if (service == null)
        {
            throw new InvalidOperationException("Servico nao encontrado ou nao pertence ao prestador");
        }

        service.Nome = request.Title;
        service.Description = request.Description;
        service.Preco = request.Price;
        service.DurationMinutes = request.DurationMinutes;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Servico atualizado: {ServiceId}", serviceId);

        return MapToDto(service);
    }

    public async Task<bool> DeleteServiceAsync(Guid serviceId, Guid providerId)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.UserId == providerId);

        if (service == null)
        {
            _logger.LogWarning("Tentativa de deletar servico inexistente: {ServiceId}", serviceId);
            return false;
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Servico deletado: {ServiceId}", serviceId);

        return true;
    }

    private static ServiceDto MapToDto(Service service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Nome = service.Nome,
            Description = service.Description,
            Preco = service.Preco,
            DurationMinutes = service.DurationMinutes
        };
    }
}
