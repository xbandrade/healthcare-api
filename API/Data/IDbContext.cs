using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Data;

public interface IDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Appointment> Appointments { get; set; }
    IEnumerable<StaffMember> StaffMembers => [.. Users.OfType<StaffMember>()];
    IEnumerable<Patient> Patients => [.. Users.OfType<Patient>()];
    IEnumerable<StaffMember> Doctors => [.. Users.OfType<Doctor>()];
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
