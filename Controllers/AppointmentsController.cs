using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly HealthcareDBContext _context;

    public AppointmentsController(HealthcareDBContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAppointments()
    {
        var appointments = await _context.Appointments.ToListAsync();
        if (!appointments.Any())
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
        var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);
        if (doctor == null)
        {
            doctor = new Doctor();
            _context.Doctors.Add(doctor);
        }
        appointment.Doctor = doctor;
        appointment.DoctorName = appointment.Doctor.Name;
        var patient = await _context.Patients.FindAsync(appointment.PatientId);
        if (patient == null)
        {
            patient = new Patient();
            _context.Patients.Add(patient);
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
