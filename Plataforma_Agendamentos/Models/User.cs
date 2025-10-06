using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string UserType { get; set; } = string.Empty; // Voltando temporariamente ao singular

    // Perfil público (só para prestadores)
    [MaxLength(100)]
    public string? Slug { get; set; }
    
    [MaxLength(100)]
    public string? DisplayName { get; set; }
    
    public string? LogoUrl { get; set; }
    
    public string? CoverImageUrl { get; set; }
    
    [MaxLength(10)]
    public string? PrimaryColor { get; set; }
    
    public string? Bio { get; set; }

    // Navigation properties
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    
    // Métodos helpers para compatibilidade com multi-role
    public bool IsCliente() => UserType == "cliente" || UserType.Contains("cliente");
    public bool IsPrestador() => UserType == "prestador" || UserType.Contains("prestador");
    public bool HasRole(string role) => UserType == role || UserType.Contains(role);
    
    public List<string> GetRoles()
    {
        // Se for JSON, tentar deserializar
        if (UserType.StartsWith("[") && UserType.EndsWith("]"))
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(UserType) ?? new List<string> { UserType };
            }
            catch
            {
                return new List<string> { UserType };
            }
        }
        
        // Se for string simples, retornar como array
        return new List<string> { UserType };
    }
    
    public void SetRoles(List<string> roles)
    {
        // Por enquanto, usar apenas o primeiro role
        UserType = roles.FirstOrDefault() ?? "cliente";
    }
}