using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HealthcareAPI.Data;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration configuration, BaseDBContext context) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly BaseDBContext _context = context;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO request)
    {
        var user = GetUserByUsername(request.Username ?? "");
        if (user != null && user.VerifyPassword(request.Password ?? ""))
        {
            return Ok(AuthManager.GenerateToken(user, _configuration));
        }
        return Unauthorized();
    }

    [Authorize]
    [HttpGet("verify")]
    public IActionResult VerifyToken()
    {
        return Ok(new { message = "Token is valid and active." });
    }

    private StaffMember? GetUserByUsername(string username)
    {
        return _context.StaffMembers.FirstOrDefault(s => s.Username == username);
    }
}
