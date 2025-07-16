using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace EfCoreContext.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherData>
{
    public void Configure(EntityTypeBuilder<WeatherData> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.Date, e.CityId }).IsUnique();
        builder.HasIndex(e=>e.CityId);
        builder.HasIndex(x => x.Latitude);
        builder.HasIndex(x => x.Longitude);
        builder.HasIndex(x=>new {x.Latitude, x.Longitude});
        builder.Property(e => e.Summary).HasConversion(
            v => v.ToString(),
            v => Enum.Parse<WeatherWmoCode>(v));
    }
}
