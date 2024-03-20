using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI;

public class HealthcareDBContext(DbContextOptions<HealthcareDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=healthcare.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasBaseType<User>();
        modelBuilder.Entity<StaffMember>().HasBaseType<User>();
        modelBuilder.Entity<Doctor>().HasBaseType<StaffMember>();
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId);
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId);
        base.OnModelCreating(modelBuilder);
    }
}
