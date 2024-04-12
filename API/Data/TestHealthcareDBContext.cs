using Microsoft.EntityFrameworkCore;
using Moq;
using HealthcareAPI.Helper;

namespace HealthcareAPI.Data;

public class TestHealthcareDBContext : BaseDBContext
{
    public override DbSet<User> Users { get; set; } = new Mock<DbSet<User>>().Object;
    public override DbSet<Appointment> Appointments { get; set; } = new Mock<DbSet<Appointment>>().Object;
    public override IEnumerable<StaffMember> StaffMembers => Users.OfType<StaffMember>();
    public override IEnumerable<Patient> Patients => Users.OfType<Patient>();
    public override IEnumerable<Doctor> Doctors => Users.OfType<Doctor>();

    public TestHealthcareDBContext() : base(new DbContextOptions<TestHealthcareDBContext>())
    {
    }

    public TestHealthcareDBContext(DbContextOptions<TestHealthcareDBContext> options) : base(options)
    {
    }
}
