namespace HealthcareAPI;

public class Doctor : StaffMember
{
    public string? Specialization { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = [];
}
