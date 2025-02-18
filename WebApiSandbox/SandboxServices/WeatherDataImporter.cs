using System.Diagnostics;
using EfCoreContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using SandboxRemoteApisImportersInterfaces;

namespace SandboxServices;

public class WeatherDataImporter : BackgroundService
{
    private readonly        ILogger<WeatherDataImporter> _logger;
    private readonly        IServiceProvider             _serviceProvider;
    private static readonly ActivitySource               s_activitySource   = new("WeatherDataImporter");
    public WeatherDataImporter(ILogger<WeatherDataImporter> logger, IServiceProvider serviceProvider)
    {
        _logger          = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WeatherDataImporter is starting");

        stoppingToken.Register(() => _logger.LogInformation("WeatherDataImporter is stopping"));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("WeatherDataImporter is running");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SandboxContext>();
                var api       = scope.ServiceProvider.GetRequiredService<IWeatherImport>();
                using var importActivity=s_activitySource.StartActivity("Importing weather data");
                try
                {
                    var startDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-Random.Shared.Next(365*60));
                    var days      = Random.Shared.Next(30);
                    var randomCity = await dbContext.Cities
                                                    .Where(x => x.Latitude >= -90 && x.Latitude < 90)
                                                    .OrderBy(c => c.WeatherForecasts.Count)
                                                    .FirstOrDefaultAsync(stoppingToken);
                    var roundedLatitude  = Math.Round(randomCity.Latitude,  4);
                    var roundedLongitude = Math.Round(randomCity.Longitude, 4);
                    var forecasts = await api.GetForecasts(roundedLatitude, roundedLongitude,
                                                           startDate, days, stoppingToken);
                    using var importDbActivity=s_activitySource.StartActivity("Saving forecasts to db", ActivityKind.Producer);
                    await dbContext.WeatherForecasts.AddRangeAsync(forecasts.Select(f => new WeatherForecast
                                                                       {
                                                                           CityId = randomCity.Id,
                                                                           Date   = f.Date,
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
}