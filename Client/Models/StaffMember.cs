using System.Text.Json.Serialization;
namespace Client;

public class StaffMember : User
{
    public string? Status { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}
