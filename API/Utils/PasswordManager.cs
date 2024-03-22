using System.Security.Cryptography;
using System.Text;

namespace HealthcareAPI;

public class PasswordManager {
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
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
        Array.Copy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
        Array.Copy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);
        byte[] hashedBytes = SHA256.HashData(combinedBytes);
        return Convert.ToBase64String(hashedBytes);
    }

    public static bool VerifyPassword(string password, string salt, string hash)
    {
        string hashedPassword = HashPassword(password, salt);
        return hash == hashedPassword;
    }
}
