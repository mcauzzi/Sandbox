using EfCoreContext.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreContext;

public class SandboxContext : DbContext
{
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    public DbSet<City>            Cities           { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=sandbox;Username=sandbox;Password=sandbox123");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>();
    }
}