using WeatherImportConfigs;
using WebApiSandboxViewModels;

namespace SandboxRemoteApisImportersInterfaces;

public interface IWeatherImport
{
    public Task<List<ForecastViewModel>> GetForecasts(OpenMeteoImporterConfig config, CancellationToken ct);
}