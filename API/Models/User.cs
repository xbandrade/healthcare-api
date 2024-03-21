using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthcareAPI;

public abstract class User
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name field cannot be empty")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Email field cannot be empty")]
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int Age { get; set; }
    public string? Gender { get; set; }
}