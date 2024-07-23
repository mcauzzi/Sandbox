using EfCoreContext.Models;

namespace DbDataGenerator;

public class State
{
    public string        Name   { get; set; }
    public HashSet<City> Cities { get; set; }
}