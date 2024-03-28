using System.ComponentModel.DataAnnotations;

namespace HealthcareAPI;
public class LoginDTO
{
    [Required(ErrorMessage = "Username cannot be empty")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "Password cannot be empty")]
    public string? Password { get; set; }
}