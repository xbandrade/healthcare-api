using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
