using Plataforma_Agendamentos.DTOs.Schedule;
using Plataforma_Agendamentos.DTOs.Service;

namespace Plataforma_Agendamentos.DTOs.Prestador;

/// <summary>
/// DTO para perfil publico de prestador
/// </summary>
public class PrestadorPublicDto
{
    public string Slug { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? TituloProfissional { get; set; }
    public string? Bio { get; set; }
    public string? Site { get; set; }
    public string? Telefone { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public IEnumerable<ServiceDto> Services { get; set; } = new List<ServiceDto>();
    public IEnumerable<ScheduleDto> Schedules { get; set; } = new List<ScheduleDto>();
}
