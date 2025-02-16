using System.Collections.Generic;

namespace Models;

public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public HashSet<State> States { get; set; } = new();
}
