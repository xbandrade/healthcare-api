using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("patients")]
public class PatientsController(HealthcareDBContext context) : ControllerBase
{
    private readonly HealthcareDBContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _context.Users.OfType<Patient>().ToListAsync();
        if (patients.Count == 0)
        {
            return NotFound();
        }
        return Ok(patients);
    }

    [HttpGet("{id}", Name = "GetPatient")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var patient = await _context.Users.OfType<Patient>().FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }
        return Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] Patient patient)
    {
        if (!ModelState.IsValid || patient.Password is null)
        {
            return BadRequest(ModelState);
        }
        patient.SetPassword(patient.Password);
        _context.Users.Add(patient);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetPatient", new { id = patient.Id }, patient);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {    
        var patient = await _context.Users.OfType<Patient>().FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }
        _context.Users.Remove(patient);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
