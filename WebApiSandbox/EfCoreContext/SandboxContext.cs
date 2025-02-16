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

        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Date, e.CityId }).IsUnique();
            entity.Property(e => e.Summary).HasConversion(
                v => v.ToString(),
                v => (WeatherWmoCode)Enum.Parse(typeof(WeatherWmoCode), v));
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StateId);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.HasOne(e => e.State).WithMany(s => s.Cities).HasForeignKey(e => e.StateId);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.HasOne(e => e.Country).WithMany(c => c.States).HasForeignKey(e => e.CountryId);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
        });
    }
}
