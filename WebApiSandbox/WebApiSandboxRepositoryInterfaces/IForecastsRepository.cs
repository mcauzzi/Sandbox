using WebApiSandboxViewModels;

namespace WebApiSandboxRepositoryInterfaces;

public interface IForecastsRepository : IStandardRepository<ForecastViewModel>
{
    public Task<IEnumerable<ForecastViewModel>> GetByCity(int       cityId,  int rows, int offset);
    public Task<IEnumerable<ForecastViewModel>> GetByDate(DateOnly  date,    int rows, int offset);
    public Task<IEnumerable<ForecastViewModel>> GetBySummary(string summary, int rows, int offset);
}