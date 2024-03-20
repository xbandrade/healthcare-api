namespace HealthcareAPI;

public class Doctor : StaffMember
{
    public string Specialization { get; set; } = "Specialization";
    public ICollection<Appointment> Appointments { get; set; } = [];
}
