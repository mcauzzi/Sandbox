namespace WebApiSandboxViewModels;

public class ForecastViewModel
{
    public string   CityName       { get; init; }
    public string   StateName      { get; init; }
    public string   CountryName    { get; init; }
    public string   Summary        { get; init; }
    public decimal  Temperature    { get; init; }
    public DateTime Date           { get; init; }
    public decimal  TemperatureMax { get; set; }
    public decimal  TemperatureMin { get; set; }
}