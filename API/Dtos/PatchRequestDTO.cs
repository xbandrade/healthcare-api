namespace HealthcareAPI;

public class PatchRequestDTO
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Status { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string? Specialization { get; set; }
    public int Age { get; set; }
}