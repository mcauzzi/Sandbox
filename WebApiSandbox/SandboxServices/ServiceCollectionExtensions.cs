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
        foreach (var section in openMeteoSections.Get<List<OpenMeteoImporterConfig>>())
        {
            services.Configure<OpenMeteoImporterConfig>(openMeteoSections);
            services.AddHostedService<WeatherDataApiImporter>();
            services.AddHttpClient<IWeatherImport, OpenMeteoHistoricalImporter>(x=>x.BaseAddress= new Uri("https://api.open-meteo.com/v1/"));
        }
        
       
        return services;
    }
}