using System.ComponentModel.DataAnnotations;

namespace HealthcareAPI;

public class PatientDTO
{
    [Required(ErrorMessage = "Name field cannot be empty")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Email field cannot be empty")]
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
    [Required(ErrorMessage = "Birth Date field cannot be empty")]
    public DateTime? BirthDate { get; set; }
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? AdditionalInfo { get; set; }
}
