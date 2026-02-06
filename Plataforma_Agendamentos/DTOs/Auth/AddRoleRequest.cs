using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs.Auth;

public class AddRoleRequest
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
