using System.Collections.Generic;

namespace Models;

public class City
{
    public          int                      Id               { get; set; }
    public required string                   Name             { get; set; }
    public required int                      StateId          { get; set; }
    public          string?                  Description      { get; set; }
    public          decimal                  Latitude         { get; set; }
    public          decimal                  Longitude        { get; set; }
    public          State?                   State            { get; set; }
    public          HashSet<WeatherData> WeatherForecasts { get; set; } = new();
}
