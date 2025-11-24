using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SandboxRemoteApisImporters;
using SandboxRemoteApisImportersInterfaces;
using WeatherImportConfigs;

namespace SandboxServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherImporterServices(this IServiceCollection services,IConfigurationSection openMeteoSections)
    {
        foreach (var section in openMeteoSections.Get<Dictionary<string,IConfigurationSection>>())
        {
            services.Configure<OpenMeteoImporterConfig>(section.Key,section.Value);
            services.AddHostedService<WeatherDataApiImporter>(x =>
                                                              {
                                                                  var configKey = section.Key;
                                                                  return ActivatorUtilities.CreateInstance<WeatherDataApiImporter>(x, configKey);
                                                              });
            services.AddHttpClient<IWeatherImport, OpenMeteoHistoricalImporter>(x=>x.BaseAddress= new Uri("https://archive-api.open-meteo.com/v1/"));
        }
        
       
        return services;
    }
}