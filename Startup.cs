using Microsoft.OpenApi.Models;

namespace HealthcareAPI;
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v0", new OpenApiInfo { Title = "Healthcare API", Version = "v0" });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            c.EnableAnnotations();
        });
        services.AddDbContext<HealthcareDBContext>();
        services.AddControllers();
        services.AddEndpointsApiExplorer();   
        services.AddAuthorization();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Healthcare API v0"));
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseDeveloperExceptionPage();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
