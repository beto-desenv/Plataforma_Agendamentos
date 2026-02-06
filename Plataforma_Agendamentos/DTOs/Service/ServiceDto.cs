namespace Plataforma_Agendamentos.DTOs.Service;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Preco { get; set; }
    public int DurationMinutes { get; set; }
}
