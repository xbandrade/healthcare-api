namespace HealthcareAPI;

public class Patient : User
{
    public DateTime BirthDate { get; set; } = DateTime.Now;
    public DateTime PatientSince { get; set; } = DateTime.Now;
    public ICollection<Appointment> Appointments { get; set; } = [];
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? AdditionalInfo { get; set; }
}
