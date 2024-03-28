using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthcareAPI;

public class AccountDTO : LoginDTO
{
    public string? Status { get; set; }
    [Required(ErrorMessage = "Name field cannot be empty")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Email field cannot be empty")]
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string? Specialization { get; set; }
    public int Age { get; set; }
}