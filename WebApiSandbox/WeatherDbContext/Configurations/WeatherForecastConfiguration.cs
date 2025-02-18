using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace EfCoreContext.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
{
    public void Configure(EntityTypeBuilder<WeatherForecast> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.Date, e.CityId }).IsUnique();
        builder.HasIndex(e=>e.CityId);
        builder.Property(e => e.Summary).HasConversion(
            v => v.ToString(),
            v => (WeatherWmoCode)Enum.Parse(typeof(WeatherWmoCode), v));
    }
}
