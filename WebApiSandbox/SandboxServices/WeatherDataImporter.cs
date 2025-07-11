using System.Diagnostics;
using EfCoreContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.Views;
using SandboxRemoteApisImportersInterfaces;
using WebApiSandboxViewModels;

namespace SandboxServices;

public class WeatherDataImporter : BackgroundService
{
    private readonly        ILogger<WeatherDataImporter> _logger;
    private readonly        IServiceProvider             _serviceProvider;
    private static readonly ActivitySource               s_activitySource   = new("WeatherDataImporter");
    public WeatherDataImporter(ILogger<WeatherDataImporter> logger, IServiceProvider serviceProvider,IOptions<>)
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
                    var    forecasts        = await GetForecastFromApi(api,stoppingToken);
                    using var importDbActivity =s_activitySource.StartActivity("Saving forecasts to db", ActivityKind.Producer);
                    if (Random.Shared.Next(10) == 1)
                    {
                        _logger.LogInformation("Refreshing materialized view {MaterializedView}", "RandomCityView");
                        await dbContext.Database.ExecuteSqlAsync($"Refresh Materialized View \"RandomCityView\"", stoppingToken);
                    }
                   
                    await dbContext.WeatherForecasts.AddRangeAsync(forecasts.Select(f => new WeatherForecast
                                                                       {
                                                                           CityId = randomCity.CityId,
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

    private static async Task<List<ForecastViewModel>> GetForecastFromApi(IWeatherImport    api,CancellationToken stoppingToken)
    {
        var forecasts = await api.GetForecasts(stoppingToken);
        return forecasts;
    }
}