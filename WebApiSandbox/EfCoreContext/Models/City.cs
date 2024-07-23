using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EfCoreContext.Models;

[Index(nameof(Country))]
[Index(nameof(State))]
public class City
{
    [Key]
    public int Id { get;              set; }
    public string  Name        { get; set; }
    public string  Country     { get; set; }
    public string  State       { get; set; }
    public string? Description { get; set; }
    
    public ICollection<WeatherForecast> WeatherForecasts { get; set; }
}