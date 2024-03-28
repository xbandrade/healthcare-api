using System.Text.Json.Serialization;

namespace HealthcareAPI;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string? PatientName { get; set; }
    public int DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public string? Title { get; set; }
    public DateTime BookingDate { get; private set; } = DateTime.Now;
    public DateTime AppointmentDate { get; set; }
    public bool IsCompleted { get; set; } = false;
    public string? Details { get; set; }
    public string? Results { get; set; }
}