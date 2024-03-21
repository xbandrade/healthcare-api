using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[Authorize]
[ApiController]
[Route("staff")]
public class StaffController(HealthcareDBContext context) : ControllerBase
{
    private readonly HealthcareDBContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllStaffMembers()
    {
        Console.WriteLine("hi");
        var staffMembers = await _context.Users.OfType<StaffMember>().ToListAsync();
        if (staffMembers.Count == 0)
        {
            return NotFound();
        }
        return Ok(staffMembers);
    }

    [HttpGet("{id}", Name = "GetStaffMember")]
    public async Task<IActionResult> GetStaffMember(int id)
    {
        var staffMember = await _context.Users.OfType<StaffMember>().FirstOrDefaultAsync(s => s.Id == id);
        if (staffMember == null)
        {
            return NotFound();
        }
        return Ok(staffMember);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaffMember([FromBody] StaffMember staffMember)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        staffMember.SetPassword();
        _context.Users.Add(staffMember);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetStaffMember", new { id = staffMember.Id }, staffMember);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaffmember(int id)
    {    
        var staff = await _context.Users.OfType<StaffMember>().FirstOrDefaultAsync(s => s.Id == id);
        if (staff == null)
        {
            return NotFound();
        }
        _context.Users.Remove(staff);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
