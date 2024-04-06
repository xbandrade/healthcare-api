namespace Client;

public class Patient : User
{
    public DateTime? BirthDate { get; set; }
    public DateTime? PatientSince { get; set; }
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? AdditionalInfo { get; set; }
}
