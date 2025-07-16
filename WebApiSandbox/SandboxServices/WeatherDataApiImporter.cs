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
    private readonly        IServiceProvider             _serviceProvider;
    private static readonly ActivitySource               s_activitySource = new("WeatherDataApiImporter");

    public WeatherDataApiImporter(ILogger<WeatherDataApiImporter> logger, IServiceProvider serviceProvider, IOptions<OpenMeteoImporterConfig> config)
        : base()
    {
        _logger          = logger;
        _serviceProvider = serviceProvider;
        Config           = config.Value;
    }

    private OpenMeteoImporterConfig Config { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WeatherDataApiImporter is starting");

        stoppingToken.Register(() => _logger.LogInformation("WeatherDataApiImporter is stopping"));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("WeatherDataApiImporter is running");

            using (var scope = _serviceProvider.CreateScope())
            {
                var       dbContext      = scope.ServiceProvider.GetRequiredService<SandboxContext>();
                var       api            = scope.ServiceProvider.GetRequiredService<IWeatherImport>();
                using var importActivity = s_activitySource.StartActivity("Importing weather data");
                try
                {
                    var forecasts = await GetForecastFromApi(api, stoppingToken);
                    using var importDbActivity =
                        s_activitySource.StartActivity("Saving Weather Data to db", ActivityKind.Producer);

                    await dbContext.WeatherForecasts.AddRangeAsync(forecasts.Select(f => new WeatherData
                                                                       {
                                                                           Latitude=Config.Latitude,
                                                                           Longitude=Config.Longitude,
                                                                               Date= f.Date,
                                                                           Summary = Enum
                                                                               .Parse<
                                                                                   WeatherWmoCode>(f
                                                                                   .Summary),
                                                                           TemperatureC = f.Temperature
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
        IWeatherImport api, CancellationToken stoppingToken)
    {
        var forecasts = await api.GetForecasts(stoppingToken);
        return forecasts;
    }
}