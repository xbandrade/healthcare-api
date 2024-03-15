namespace HealthcareAPI;

public class Patient {
    public int Id { get; set; }
    public string Name { get; set; } = "Patient Name";
    public int Age { get; set; }
    
    public string Gender { get; set; } = "Gender";
    public string Address { get; set; } = "Full Address";
    public string Phone { get; set; } = "+5513123128";
    public string Email { get; set; } = "Email Address";
    public string Password { get; set; } = "Password";
    public DateTime BirthDate { get; set; } = DateTime.Now;
    public DateTime PatientSince { get; set; } = DateTime.Now;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? AdditionalInfo { get; set; }
}