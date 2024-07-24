using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EfCoreContext.Models;

[Index(nameof(StateId))]
public class City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(200)]
    public required string Name { get; set; }

    public required int StateId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
    public State? State { get; set; }

    public HashSet<WeatherForecast> WeatherForecasts { get; set; } = new();
}