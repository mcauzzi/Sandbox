﻿using EfCoreContext.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        //modelBuilder.HasPostgresEnum<WeatherSummary>();
        modelBuilder
            .Entity<WeatherForecast>()
            .Property(e => e.Summary)
            .HasConversion(
                           v => v.ToString(),
                           v => (WeatherWmoCode)Enum.Parse(typeof(WeatherWmoCode), v));
        //modelBuilder.Entity<WeatherForecast>().Property(wf => wf.Summary).HasConversion<int>();
    }
}