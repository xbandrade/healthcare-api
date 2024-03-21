using System.ComponentModel.DataAnnotations;

namespace HealthcareAPI;

public class StaffMember : User
{
    public string Status { get; set; } = "Active";
    [Required(ErrorMessage = "Username field cannot be empty")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "Password field cannot be empty")]
    public string? Password { get; set; }  // Password is stored after being hashed with salt
    protected string? Salt { get; private set; }

    public void SetPassword()
    {
        (Salt, Password) = PasswordManager.SetPassword(Password ?? "");
    }

    public bool VerifyPassword(string password)
    {
        if (Password is null || Salt is null)
        {
            return false;
        }
        return PasswordManager.VerifyPassword(password, Salt, Password);
    }

    public bool IsPasswordValid()
    {
        // ...
        return !string.IsNullOrEmpty(Password);
    }
}
