namespace WebApiSandboxViewModels;

public class ForecastViewModel
{
    public string   Summary        { get; init; }
    public decimal  Temperature    { get; init; }
    public DateTime Date           { get; init; }
    public decimal  TemperatureMax { get; set; }
    public decimal  TemperatureMin { get; set; }
    public decimal  Latitude       { get; set; }
    public decimal  Longitude      { get; set; }
}