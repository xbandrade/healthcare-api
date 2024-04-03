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
        var doctors = await _context.Users
            .OfType<Doctor>()
            .Include(d => d.Appointments)
            .ToListAsync();
        if (doctors.Count == 0)
        {
            return NotFound();
        }
        return Ok(doctors);
    }

    [HttpGet("{id}", Name = "GetDoctor")]
    public async Task<IActionResult> GetDoctor(int id)
    {
        var doctor = await _context.Users
            .OfType<Doctor>()
            .Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (doctor is null)
        {
            return NotFound();
        }
        return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] AccountDTO accountDTO)
    {
        if (!ModelState.IsValid || accountDTO is null)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        var doctor = new Doctor
        {
            Username = accountDTO.Username,
            Name = accountDTO.Name,
            Email = accountDTO.Email,
            Phone = accountDTO.Phone,
            Address = accountDTO.Address,
            Status = accountDTO.Status,
            Gender = accountDTO.Gender,
            Specialization = accountDTO.Specialization,
            Age = accountDTO.Age
        };
        doctor.SetPassword(accountDTO.Password ?? "");
        _context.Users.Add(doctor);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetDoctor", new { id = doctor.Id }, doctor);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateDoctor(int id, [FromBody] PatchRequestDTO accountDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        var doctor = await _context.Users.OfType<Doctor>().FirstOrDefaultAsync(d => d.Id == id);
        if (doctor is null)
        {
            return NotFound("Doctor not found");
        }
        foreach (var property in accountDTO.GetType().GetProperties())
        {
            var type = property.PropertyType;
            var value = property.GetValue(accountDTO);
            if (value is not null)
            {        
                var doctorProperty = doctor.GetType().GetProperty(property.Name);
                if (doctorProperty is not null && doctorProperty.PropertyType == type &&
                    (type != typeof(int) || (int)value != 0))
                {
                    doctorProperty.SetValue(doctor, value);
                }
            }
        }
        var passwordChanged = !string.IsNullOrEmpty(accountDTO.Password);
        if (passwordChanged)
        {
            doctor.SetPassword(accountDTO.Password);
        }
        try
        {
            _context.Update(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound($"Failed to update Doctor id {id}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {    
        var doctor = await _context.Users.OfType<Doctor>().FirstOrDefaultAsync(d => d.Id == id);
        if (doctor is null)
        {
            return NotFound();
        }
        _context.Users.Remove(doctor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
