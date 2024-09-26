namespace SandboxConfigurations;
//https://archive-api.open-meteo.com/v1/archive?latitude=52.52&longitude=13.41&start_date=2024-09-10&end_date=2024-09-24&daily=weather_code,temperature_2m_max,temperature_2m_min
public record OpenMeteoImporterConfig
{
    public string BaseUrl { get; init; }
    public string ApiType { get; init; }
}