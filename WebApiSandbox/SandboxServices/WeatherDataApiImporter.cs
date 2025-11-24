using System.Diagnostics;
using EfCoreContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using SandboxRemoteApisImportersInterfaces;
using WeatherImportConfigs;
using WebApiSandboxViewModels;

namespace SandboxServices;

public class WeatherDataApiImporter : BackgroundService
{
    private readonly        ILogger<WeatherDataApiImporter> _logger;
    private readonly        IServiceProvider                _serviceProvider;
    private static readonly ActivitySource                  s_activitySource = new("WeatherDataApiImporter");

    public WeatherDataApiImporter(string key, ILogger<WeatherDataApiImporter> logger, IServiceProvider serviceProvider)
    {
        _logger          = logger;
        _serviceProvider = serviceProvider;
        ConfigKey        = key;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WeatherDataApiImporter is starting");

        stoppingToken.Register(() => _logger.LogInformation("WeatherDataApiImporter is stopping"));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("WeatherDataApiImporter is running");

            using (var scope = _serviceProvider.CreateScope())
            {
                var config = scope.ServiceProvider.GetService<IOptionsSnapshot<OpenMeteoImporterConfig>>()
                                  .Get(ConfigKey);
                var       dbContext      = scope.ServiceProvider.GetRequiredService<SandboxContext>();
                var       api            = scope.ServiceProvider.GetRequiredService<IWeatherImport>();
                using var importActivity = s_activitySource.StartActivity("Importing weather data");
                try
                {
                    var forecasts = await GetForecastFromApi(api, config, stoppingToken);
                    using var importDbActivity =
                        s_activitySource.StartActivity("Saving Weather Data to db", ActivityKind.Producer);

                    await dbContext.WeatherForecasts.AddRangeAsync(forecasts.Select(f => new WeatherData
                                                                       {
                                                                           Latitude  = config.Latitude,
                                                                           Longitude = config.Longitude,
                                                                           Date =
                                                                               new DateTime(DateOnly
                                                                                    .FromDateTime(f
                                                                                        .Date),
                                                                                TimeOnly
                                                                                    .FromDateTime(f
                                                                                        .Date),
                                                                                DateTimeKind.Utc),
                                                                           Summary = Enum
                                                                               .Parse<
                                                                                   WeatherWmoCode>(f
                                                                                   .Summary),
                                                                           TemperatureC = f.Temperature,
                                                                           Source       = "WeatherImport"
                                                                       }), stoppingToken);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error importing weather data");
                }
            }

            await Task.Delay(10000, stoppingToken);
        }
    }

    private static async Task<List<ForecastViewModel>> GetForecastFromApi(
        IWeatherImport api, OpenMeteoImporterConfig config, CancellationToken stoppingToken)
    {
        var forecasts = await api.GetForecasts(config, stoppingToken);
        return forecasts;
    }

    private string ConfigKey { get; init; }
}