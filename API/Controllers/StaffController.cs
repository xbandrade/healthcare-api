using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthcareAPI.Data;

namespace HealthcareAPI.Controllers;

[Authorize]
[ApiController]
[Route("staff")]
public class StaffController(BaseDBContext context) : ControllerBase
{
    private readonly BaseDBContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllStaffMembers()
    {
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
        if (staffMember is null)
        {
            return NotFound();
        }
        return Ok(staffMember);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaffMember([FromBody] StaffDTO accountDTO)
    {
        if (!ModelState.IsValid || accountDTO is null)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        var staffMember = new StaffMember
        {
            Username = accountDTO.Username,
            Name = accountDTO.Name,
            Email = accountDTO.Email,
            Phone = accountDTO.Phone,
            Address = accountDTO.Address,
            Gender = accountDTO.Gender,
            Status = accountDTO.Status,
            Age = accountDTO.Age
        };
        staffMember.SetPassword(accountDTO.Password ?? "");
        _context.Users.Add(staffMember);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetStaffMember", new { id = staffMember.Id }, staffMember);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStaffMember(int id, [FromBody] PatchRequestDTO accountDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        var staff = await _context.Users.OfType<StaffMember>().FirstOrDefaultAsync(d => d.Id == id);
        if (staff is null)
        {
            return NotFound("Staff Member not found");
        }
        foreach (var property in accountDTO.GetType().GetProperties())
        {
            var type = property.PropertyType;
            var value = property.GetValue(accountDTO);
            if (value is not null)
            {        
                var staffProperty = staff.GetType().GetProperty(property.Name);
                if (staffProperty is not null && staffProperty.PropertyType == type &&
                    (type != typeof(int) || (int)value != 0))
                {
                    staffProperty.SetValue(staff, value);
                }
            }
        }
        if (!string.IsNullOrEmpty(accountDTO.Password))
        {
            staff.SetPassword(accountDTO.Password);
        }
        try
        {
            _context.Update(staff);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound($"Failed to update Staff Member id {id}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaffmember(int id)
    {    
        var staff = await _context.Users.OfType<StaffMember>().FirstOrDefaultAsync(s => s.Id == id);
        if (staff is null)
        {
            return NotFound();
        }
        _context.Users.Remove(staff);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
