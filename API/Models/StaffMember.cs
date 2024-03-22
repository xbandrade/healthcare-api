using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthcareAPI;

public class StaffMember : User
{
    public string Status { get; set; } = "Active";
    [Required(ErrorMessage = "Username field cannot be empty")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "Password field cannot be empty")]
    public string? Password { get; set; }
    [JsonIgnore]
    private string? _salt;
    [JsonIgnore]
    public string? Salt {
        get { return _salt; }
        private set { _salt = value; }
    }

    public void SetPassword()
    {
        (Password, Salt) = PasswordManager.SetPassword(Password ?? "");
    }

    public bool VerifyPassword(string pwd)
    {
        if (Password is null || Salt is null)
        {
            Console.WriteLine("Password or salt is null");
            return false;
        }
        return PasswordManager.VerifyPassword(pwd, Salt, Password);
    }

    public bool IsPasswordValid()
    {
        return !string.IsNullOrEmpty(Password);
    }
}
