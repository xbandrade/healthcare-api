using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("patients")]
public class PatientsController : ControllerBase
{
    private readonly HealthcareDBContext _context;

    public PatientsController(HealthcareDBContext context)
    {
        _context = context;
    }
        
    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _context.Patients.ToListAsync();
        if (!patients.Any())
        {
            return NotFound();
        }
        return Ok(patients);
    }

    [HttpGet("{id}", Name = "GetPatient")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }
        return Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] Patient patient)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetPatient", new { id = patient.Id }, patient);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {    
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
