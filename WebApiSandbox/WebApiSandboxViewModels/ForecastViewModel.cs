namespace WebApiSandboxViewModels;

public class ForecastViewModel
{
    public string   CityName    { get; init; }
    public string   StateName   { get; init; }
    public string   CountryName { get; init; }
    public string   Summary     { get; init; }
    public decimal Temperature { get; init; }
    public DateOnly Date        { get; init; }
}