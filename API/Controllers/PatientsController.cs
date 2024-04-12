using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthcareAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace HealthcareAPI.Controllers;

[Authorize]
[ApiController]
[Route("patients")]
public class PatientsController(BaseDBContext context) : ControllerBase
{
    private readonly BaseDBContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _context
            .Set<Patient>()
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
    public async Task<IActionResult> CreatePatient([FromBody] PatientDTO accountDTO)
    {
        if (!ModelState.IsValid || accountDTO is null)
        {
            return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }
        var requiredProperties = typeof(PatientDTO).GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute)));
        var missingProperties = requiredProperties
            .Where(p => p.GetValue(accountDTO) == null)
            .Select(p => p.Name);
        if (missingProperties.Any())
        {
            return BadRequest($"Missing required properties: {string.Join(", ", missingProperties)}");
        }
        var patient = new Patient
        {
            Name = accountDTO.Name,
            Email = accountDTO.Email,
            Phone = accountDTO.Phone,
            Address = accountDTO.Address,
            Gender = accountDTO.Gender,
            Age = accountDTO.Age ?? -1,
            PatientSince = DateTime.Now,
            BirthDate = accountDTO.BirthDate ?? DateTime.MinValue,
            BloodGroup = accountDTO.BloodGroup,
            Allergies = accountDTO.Allergies,
            AdditionalInfo = accountDTO.AdditionalInfo,
        };
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
