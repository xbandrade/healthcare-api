using System.ComponentModel.DataAnnotations;

namespace HealthcareAPI;

public abstract class User
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = "Name";
    [Required]
    public string Username { get; set; } = "Username";
    public string Password { get; set; } =  "";  // Hashed Password
    protected string? Salt { get; private set; }
    public string Email { get; set; } = "Email";
    public string Phone { get; set; } = "+5512312323";
    public string Address { get; set; } = "Full Address";
    public int Age { get; set; }
    public string Gender { get; set; } = "Gender";

    public void SetPassword(string pwd)
    {
        (Salt, Password) = PasswordManager.SetPassword(pwd);
    }

    public bool VerifyPassword(string pwd)
    {
        if (Password is null || Salt is null)
        {
            return false;
        }
        return PasswordManager.VerifyPassword(pwd, Salt, Password);
    }
}