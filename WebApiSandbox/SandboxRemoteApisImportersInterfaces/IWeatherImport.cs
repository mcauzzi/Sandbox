using WebApiSandboxViewModels;

namespace SandboxRemoteApisImportersInterfaces;

public interface IWeatherImport
{
    public Task<List<ForecastViewModel>> GetForecasts(decimal latitude, decimal longitude,
                                                      DateOnly endDate, int    numberOfDays, CancellationToken ct);
}