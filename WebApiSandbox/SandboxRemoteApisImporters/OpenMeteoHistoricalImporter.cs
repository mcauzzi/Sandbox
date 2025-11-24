using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using SandboxRemoteApisImportersInterfaces;
using WeatherImportConfigs;
using WebApiSandboxViewModels;

namespace SandboxRemoteApisImporters;

public class OpenMeteoHistoricalImporter : IWeatherImport
{
    public OpenMeteoHistoricalImporter(ILogger<OpenMeteoHistoricalImporter> logger, HttpClient httpClient)
    {
        Logger     = logger;
        HttpClient = httpClient;
    }

    public async Task<List<ForecastViewModel>> GetForecasts(OpenMeteoImporterConfig config, CancellationToken ct)
    {
        using var activity  = ActivitySrc.StartActivity("GetForecasts", ActivityKind.Client);
        var       startDate = DateTime.Parse(config.StartDate);
        var req = new HttpRequestMessage(HttpMethod.Get,
                                         $"archive?latitude={config.Latitude}"
                                       + $"&longitude={config.Longitude.ToString(CultureInfo.InvariantCulture)}"
                                       + $"&start_date={startDate:yyyy-MM-dd}"
                                       + $"&end_date={startDate.AddDays(config.DaysPerRequest):yyyy-MM-dd}"
                                       + $"&daily=weather_code,temperature_2m_max,temperature_2m_min");
        req.Headers.Add("Accept", "application/json");
        var response       = await HttpClient.SendAsync(req, ct);
        var mappedResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: ct);

        return mappedResponse.daily.time.Select((t, i) => new ForecastViewModel
                                                          {
                                                              Date = DateTime.Parse(t),
                                                              Summary =
                                                                  ((WeatherWmoCode)(mappedResponse.daily
                                                                                  .weather_code[i] ?? -1))
                                                                  .ToString(),
                                                              TemperatureMax =
                                                                  Convert.ToDecimal(mappedResponse.daily
                                                                      .temperature_2m_max[i] ?? -999),
                                                              TemperatureMin =
                                                                  Convert.ToDecimal(mappedResponse.daily
                                                                      .temperature_2m_min[i] ?? -999),
                                                              Temperature =
                                                                  Convert.ToDecimal(mappedResponse.daily
                                                                      .temperature_2m_max[i] ?? -999)
                                                          })
                             .ToList();
    }

    public ILogger<OpenMeteoHistoricalImporter> Logger { get; }
    public ActivitySource                       ActivitySrc = new("OpenMeteoImporter");
    public HttpClient                           HttpClient { get; }
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