using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Data;

public class HealthcareDBContext(DbContextOptions<HealthcareDBContext> options) : BaseDBContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=healthcare.db");
    }
}
