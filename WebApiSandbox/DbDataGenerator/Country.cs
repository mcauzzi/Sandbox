namespace DbDataGenerator;

public class Country
{
    public string             Name   { get; set; }
    public HashSet<State> States { get; set; }
}