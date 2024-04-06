using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[Authorize]
[ApiController]
[Route("patients")]
public class PatientsController(HealthcareDBContext context) : ControllerBase
{
    private readonly HealthcareDBContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _context.Users
            .OfType<Patient>()
            .Include(p => p.Appointments)
            .ToListAsync();
        if (patients.Count == 0)
        {
            return NotFound();
        }
        return Ok(patients);
    }

    [HttpGet("{id}", Name = "GetPatient")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var patient = await _context.Users
            .OfType<Patient>()
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (patient is null)
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
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        patient.PatientSince = DateTime.Now;
        _context.Users.Add(patient);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetPatient", new { id = patient.Id }, patient);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patientPatch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        var patient = await _context.Users.OfType<Patient>().FirstOrDefaultAsync(d => d.Id == id);
        if (patient is null)
        {
            return NotFound("Patient not found");
        }
        foreach (var property in patientPatch.GetType().GetProperties())
        {
            var type = property.PropertyType;
            var value = property.GetValue(patientPatch);
            if (value is not null)
            {        
                var staffProperty = patient.GetType().GetProperty(property.Name);
                if (staffProperty is not null && staffProperty.PropertyType == type &&
                    (type != typeof(int) || (int)value != 0))
                {
                    staffProperty.SetValue(patient, value);
                }
            }
        }
        try
        {
            _context.Update(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound($"Failed to update Patient id {id}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {    
        var patient = await _context.Users.OfType<Patient>().FirstOrDefaultAsync(p => p.Id == id);
        if (patient is null)
        {
            return NotFound();
        }
        _context.Users.Remove(patient);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
