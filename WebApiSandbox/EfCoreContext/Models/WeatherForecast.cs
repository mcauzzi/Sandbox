using System.ComponentModel.DataAnnotations;

namespace EfCoreContext.Models;

public class WeatherForecast
{
    [Key]
    public int Id { get;                set; }
    public DateOnly Date         { get; set; }
    public int      TemperatureC { get; set; }
    public string?  Summary      { get; set; }
    public int      CityId       { get; set; }
    public City     City         { get; set; }
}