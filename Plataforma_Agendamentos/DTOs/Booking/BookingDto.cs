namespace Plataforma_Agendamentos.DTOs.Booking;

public class BookingDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public ClientInfoDto Client { get; set; } = null!;
    public ServiceInfoDto Service { get; set; } = null!;
}

public class ClientInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ServiceInfoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DurationMinutes { get; set; }
    public ProviderInfoDto Provider { get; set; } = null!;
}

public class ProviderInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
