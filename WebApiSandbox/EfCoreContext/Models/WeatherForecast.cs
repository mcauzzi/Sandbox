using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EfCoreContext.Models;

[Index(nameof(Date), nameof(CityId), IsUnique = true)]
public class WeatherForecast
{
    [Key]
    public long Id { get; set; }

    public DateOnly       Date         { get; set; }
    public decimal            TemperatureC { get; set; }
    public WeatherSummary Summary      { get; set; }
    public int            CityId       { get; set; }
    public City?          City         { get; set; }
}

public enum WeatherSummary
{
    Sunny,
    Cloudy,
    Rainy,
    Snowy,
    Stormy,
    Unknown = -1
}