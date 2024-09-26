using System.Globalization;
using System.Net.Http.Json;
using EfCoreContext.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SandboxConfigurations;
using SandboxRemoteApisInterfaces;
using WebApiSandboxViewModels;

namespace SandboxRemoteApisRepositories;

public class OpenMeteoImporter : IWeatherImport
{
    public OpenMeteoImporter(ILogger<OpenMeteoImporter>        logger, HttpClient httpClient,
                             IOptions<OpenMeteoImporterConfig> config)
    {
        Logger     = logger;
        HttpClient = httpClient;
        Config     = config.Value;
    }

    public OpenMeteoImporterConfig Config { get; set; }

    public async Task<List<ForecastViewModel>> GetForecasts(decimal latitude,     decimal longitude, DateOnly endDate,
                                                            int     numberOfDays, CancellationToken ct)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
                                         $"{Config.BaseUrl}/archive?latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}&start_date={endDate.AddDays(-numberOfDays).ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&daily=weather_code,temperature_2m_max,temperature_2m_min");
        req.Headers.Add("Accept", "application/json");
        var response       = await HttpClient.SendAsync(req, ct);
        var mappedResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: ct);

        return mappedResponse.daily.time.Select((t, i) => new ForecastViewModel
                                                          {
                                                              Date = DateOnly.Parse(t),
                                                              Summary =
                                                                  ((WeatherWmoCode)(mappedResponse.daily
                                                                          .weather_code[i]??-1)).ToString(),
                                                              Temperature =
                                                                  Convert.ToDecimal(mappedResponse.daily
                                                                      .temperature_2m_max[i]??-999)
                                                          })
                             .ToList();
    }

    public ILogger<OpenMeteoImporter> Logger     { get; }
    public HttpClient                 HttpClient { get; }
}

public record ApiResponse(
    double      latitude,
    double      longitude,
    double      generationtime_ms,
    int         utc_offset_seconds,
    string      timezone,
    string      timezone_abbreviation,
    double      elevation,
    Daily_units daily_units,
    Daily       daily);

public record Daily_units(string time, string weather_code, string temperature_2m_max, string temperature_2m_min);

public record Daily(string[] time, int?[] weather_code, double?[] temperature_2m_max, double?[] temperature_2m_min);