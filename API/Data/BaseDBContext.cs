using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Data;

public class BaseDBContext(DbContextOptions options) : DbContext(options), IDbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual IEnumerable<StaffMember> StaffMembers => [.. Users.OfType<StaffMember>()];
    public virtual IEnumerable<Doctor> Doctors => [.. Users.OfType<Doctor>()];
    public virtual IEnumerable<Patient> Patients => [.. Users.OfType<Patient>()];

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Patient>().HasBaseType<User>();
        modelBuilder.Entity<StaffMember>().HasBaseType<User>();
        modelBuilder.Entity<Doctor>().HasBaseType<StaffMember>();
        modelBuilder.Entity<Appointment>()
            .HasOne<Doctor>()
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId);
        modelBuilder.Entity<Appointment>()
            .HasOne<Patient>()
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId);
        modelBuilder.Entity<StaffMember>()
            .HasIndex(s => s.Username)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
