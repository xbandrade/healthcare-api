using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace HealthcareAPI;

public class AuthManager
{
    public static string GenerateToken(StaffMember user, IConfiguration config)
    {
        var key = config["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(key)) 
        {
            throw new Exception("JWT Secret Key not found in app settings");
        }
        if (string.IsNullOrEmpty(user.Username))
        {
            throw new Exception("Invalid username");
        }
        var audience = config["Jwt:Audience"] ?? "";
        var issuer = config["Jwt:Issuer"] ?? "";
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(JwtRegisteredClaimNames.Aud, audience),
            new(JwtRegisteredClaimNames.Iss, issuer)
        };
        var tokenExpiration = TimeSpan.FromMinutes(60);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(tokenExpiration),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
