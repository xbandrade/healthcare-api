using System.Text.Json.Serialization;
namespace HealthcareAPI;

public class StaffMember : User
{
    public string? Status { get; set; }
    public string? Username { get; set; }
    private string? _hashedPassword;
    [JsonIgnore]
    public string? HashedPassword {
        get { return _hashedPassword; }
        private set { _hashedPassword = value; }
    }
    private string? _salt;
    [JsonIgnore]
    public string? Salt {
        get { return _salt; }
        private set { _salt = value; }
    }

    public void SetPassword(string password)
    {
        (HashedPassword, Salt) = PasswordManager.SetPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        if (HashedPassword is null || Salt is null)
        {
            Console.WriteLine("Password or salt is null");
            return false;
        }
        return PasswordManager.VerifyPassword(password, Salt, HashedPassword);
    }

    public bool IsPasswordValid()
    {
        return !string.IsNullOrEmpty(HashedPassword);
    }
}
