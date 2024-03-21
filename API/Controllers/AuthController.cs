using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration configuration, HealthcareDBContext context) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly HealthcareDBContext _context = context;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        Console.WriteLine($"Username: {request.Username}");
        Console.WriteLine($"Password: {request.Password}");
        var user = GetUserByUsername(request.Username);
        Console.WriteLine($"Name: {user?.Name}");
        if (user != null && !user.VerifyPassword(request.Password))
        {
            return Ok(GenerateToken(user));
        }
        return Unauthorized();
    }

    private string GenerateToken(StaffMember user)
    {
        var key = _configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(key)) 
        {
            throw new Exception("JWT Secret Key not found in app settings");
        }
        if (string.IsNullOrEmpty(user.Username))
        {
            throw new Exception("Invalid username");
        }
        var audience = _configuration["Jwt:Audience"] ?? "";
        var issuer = _configuration["Jwt:Issuer"] ?? "";
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

    [Authorize]
    [HttpGet("verify")]
    public IActionResult VerifyToken()
    {
        return Ok(new { message = "Token is valid and active." });
    }

    private StaffMember? GetUserByUsername(string username)
    {
        return _context.Users.OfType<StaffMember>().FirstOrDefault(u => u.Username == username);
    }
}
