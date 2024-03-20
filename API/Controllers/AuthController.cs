/*using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration configuration, HealthcareDBContext context) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly HealthcareDBContext _context = context;

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = GetUserByUsername(request.Username);
        if (user != null && user.VerifyPassword(request.Password))
        {
            return Ok(GenerateToken(user));
        }
        return Unauthorized();
    }

    private string GenerateToken(User user)
    {
        var key = _configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(key)) 
        {
            throw new Exception("JWT Secret Key not found in secrets");
        }
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
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

    private User? GetUserByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }
}
*/