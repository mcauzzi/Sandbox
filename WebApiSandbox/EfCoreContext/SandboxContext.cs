using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace EfCoreContext;

public class SandboxContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public SandboxContext(DbContextOptions<SandboxContext> options) : base(options)
    {
    }

    public SandboxContext()
    {
    }

    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    public DbSet<City>            Cities           { get; set; }
    public DbSet<State>           States           { get; set; }
    public DbSet<Country>         Countries        { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=sandbox;Username=sandbox;Password=sandbox123");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new WeatherForecastConfiguration());
        modelBuilder.ApplyConfiguration(new CityConfiguration());
        modelBuilder.ApplyConfiguration(new StateConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
    }
}