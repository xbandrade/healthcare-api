using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
namespace HealthcareAPI;

public class PasswordManager
{
    private static string GenerateSalt()
    {
        byte[] saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
    
    public static (string, string) SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception("Password cannot be empty");
        }
        string salt = GenerateSalt();
        string hashedPassword = HashPassword(password, salt);
        return (hashedPassword, salt);
    }

    private static string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        using var hasher = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = saltBytes,
            DegreeOfParallelism = 8,
            MemorySize = 65536,
            Iterations = 8
        };
        byte[] hashedBytes = hasher.GetBytes(32);
        return Convert.ToBase64String(hashedBytes);
    }

    public static bool VerifyPassword(string password, string salt, string hash)
    {
        string hashedPassword = HashPassword(password, salt);
        return hash == hashedPassword;
    }
}
