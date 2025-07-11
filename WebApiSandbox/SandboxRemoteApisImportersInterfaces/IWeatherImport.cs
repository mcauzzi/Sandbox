using WebApiSandboxViewModels;

namespace SandboxRemoteApisImportersInterfaces;

public interface IWeatherImport
{
    public Task<List<ForecastViewModel>> GetForecasts( CancellationToken ct);
}