using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("appointments")]
public class AppointmentsController(HealthcareDBContext context) : ControllerBase
{
    private readonly HealthcareDBContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllAppointments()
    {
        var appointments = await _context.Appointments.ToListAsync();
        if (appointments.Count == 0)
        {
            return NotFound();
        }
        return Ok(appointments);
    }

    [HttpGet("{id}", Name = "GetAppointment")]
    public async Task<IActionResult> GetAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }
        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var doctor = await _context.Users.OfType<Doctor>().FirstOrDefaultAsync(d => d.Id == appointment.DoctorId);
        if (doctor == null)
        {
            doctor = new();
            _context.Users.Add(doctor);
        }
        appointment.Doctor = doctor;
        appointment.DoctorName = appointment.Doctor.Name;
        var patient = await _context.Users.OfType<Patient>().FirstOrDefaultAsync(p => p.Id == appointment.DoctorId);
        if (patient == null)
        {
            patient = new();
            _context.Users.Add(patient);
        }
        appointment.Patient = patient;
        appointment.PatientName = appointment.Patient.Name;
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetAppointment", new { id = appointment.Id }, appointment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {    
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
