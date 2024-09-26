using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EfCoreContext.Models;

[Index(nameof(Date), nameof(CityId), IsUnique = true)]
public class WeatherForecast
{
    [Key]
    public long Id { get; set; }

    public DateOnly       Date         { get; set; }
    public decimal        TemperatureC { get; set; }
    public WeatherWmoCode Summary      { get; set; }
    public int            CityId       { get; set; }
    public City?          City         { get; set; }
}

public enum WeatherWmoCode
{
    Undefined  =-1,
    ClearSky              = 0,
    MainlyClear           = 1,
    PartlyCloudy          = 2,
    Overcast              = 3,
    Fog                   = 45,
    DepositingRimeFog     = 48,
    DrizzleLight          = 51,
    DrizzleModerate       = 53,
    DrizzleDense          = 55,
    FreezingDrizzleLight  = 56,
    FreezingDrizzleDense  = 57,
    RainSlight            = 61,
    RainModerate          = 63,
    RainHeavy             = 65,
    FreezingRainLight     = 66,
    FreezingRainHeavy     = 67,
    SnowFallSlight        = 71,
    SnowFallModerate      = 73,
    SnowFallHeavy         = 75,
    SnowGrains            = 77,
    RainShowersSlight     = 80,
    RainShowersModerate   = 81,
    RainShowersViolent    = 82,
    SnowShowersSlight     = 85,
    SnowShowersHeavy      = 86,
    ThunderstormSlight    = 95,
    ThunderstormHeavyHail = 96,
    ThunderstormHeavy     = 99
}