using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("doctors")]
public class DoctorsController : ControllerBase
{
    private readonly HealthcareDBContext _context;

    public DoctorsController(HealthcareDBContext context)
    {
        _context = context;
    }
        
    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        var doctors = await _context.Doctors.ToListAsync();
        if (!doctors.Any())
        {
            return NotFound();
        }
        return Ok(doctors);
    }

    [HttpGet("{id}", Name = "GetDoctor")]
    public async Task<IActionResult> GetDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
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
            return BadRequest(ModelState);
        }
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetDoctor", new { id = doctor.Id }, doctor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {    
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
        {
            return NotFound();
        }
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
