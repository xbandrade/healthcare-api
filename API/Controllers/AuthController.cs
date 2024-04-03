using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration configuration, HealthcareDBContext context) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly HealthcareDBContext _context = context;

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
        return _context.Users.OfType<StaffMember>().FirstOrDefault(u => u.Username == username);
    }
}
