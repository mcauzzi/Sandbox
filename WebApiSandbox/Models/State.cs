using System.Collections.Generic;

namespace Models;

public class State
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    public HashSet<City> Cities { get; set; } = new();
}
