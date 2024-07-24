using System.ComponentModel.DataAnnotations;

namespace EfCoreContext.Models;

public class State
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public required string Name { get; set; }

    public int           CountryId { get; set; }
    public Country?      Country   { get; set; }
    public HashSet<City> Cities    { get; set; } = new();
}