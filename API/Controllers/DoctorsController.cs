using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[Authorize]
[ApiController]
[Route("doctors")]
public class DoctorsController(HealthcareDBContext context) : ControllerBase
{
    private readonly HealthcareDBContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        var doctors = await _context.Users.OfType<Doctor>().ToListAsync();
        if (doctors.Count == 0)
        {
            return NotFound();
        }
        return Ok(doctors);
    }

    [HttpGet("{id}", Name = "GetDoctor")]
    public async Task<IActionResult> GetDoctor(int id)
    {
        var doctor = await _context.Users.OfType<Doctor>().FirstOrDefaultAsync(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound();
        }
        return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] Doctor doctor)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        doctor.SetPassword();
        _context.Users.Add(doctor);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetDoctor", new { id = doctor.Id }, doctor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {    
        var doctor = await _context.Users.OfType<Doctor>().FirstOrDefaultAsync(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound();
        }
        _context.Users.Remove(doctor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
