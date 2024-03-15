namespace HealthcareAPI;

public class Doctor {
    public int Id { get; set; }
    public string Name { get; set; } = "Doctor Name";
    public int Age { get; set; }
    public string Specialization { get; set; } = "Specialization";
    public string Email { get; set; } = "Email";
    public string Password { get; set; } = "Password";
    public string Phone { get; set; } = "+5512312323";
    public string Address { get; set; } = "Full Address";
    public string Status { get; set; } = "Active";
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
