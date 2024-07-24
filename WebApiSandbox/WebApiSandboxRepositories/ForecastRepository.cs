using EfCoreContext;
using EfCoreContext.Models;
using Microsoft.EntityFrameworkCore;
using WebApiSandboxRepositoryInterfaces;
using WebApiSandboxViewModels;

namespace WebApiSandboxRepositories;

public class ForecastRepository:IForecastsRepository
{
    public SandboxContext Context { get; }

    public ForecastRepository(SandboxContext context)
    {
        Context = context;
    }
    public async Task<IEnumerable<ForecastViewModel>> Get(int rows, int offset)
    {
        return await Context.WeatherForecasts
                      .OrderBy(x=>x.Id)
                      .Skip(offset)
                      .Take(rows)
                      .Select(wf => new ForecastViewModel
                      {
                          CityName    = wf.City.Name,
                          StateName   = wf.City.State.Name,
                          CountryName = wf.City.State.Country.Name,
                          Summary     = wf.Summary.ToString(),
                          Date        = wf.Date
                      }).ToListAsync();
    }

    public async Task<IEnumerable<ForecastViewModel>> GetByCity(int cityId, int rows, int offset)
    {
        return await Context.WeatherForecasts
                            .Where(x=>x.CityId==cityId)
                            .OrderBy(x=>x.Id)
                            .Skip(offset)
                            .Take(rows)
                            .Select(wf => new ForecastViewModel
                                          {
                                              CityName    = wf.City.Name,
                                              StateName   = wf.City.State.Name,
                                              CountryName = wf.City.State.Country.Name,
                                              Summary     = wf.Summary.ToString(),
                                              Date        = wf.Date
                                          }).ToListAsync();
    }

    public async Task<IEnumerable<ForecastViewModel>> GetByDate(DateOnly date, int rows, int offset)
    {
        return await Context.WeatherForecasts
                            .Where(x=>x.Date==date)
                            .OrderBy(x=>x.Id)
                            .Skip(offset)
                            .Take(rows)
                            .Select(wf => new ForecastViewModel
                                          {
                                              CityName    = wf.City.Name,
                                              StateName   = wf.City.State.Name,
                                              CountryName = wf.City.State.Country.Name,
                                              Summary     = wf.Summary.ToString(),
                                              Date        = wf.Date
                                          }).ToListAsync();
    }

    public async Task<IEnumerable<ForecastViewModel>> GetBySummary(string summary, int rows, int offset)
    {
        return await Context.WeatherForecasts
                            .Where(x=>x.Summary==Enum.Parse<WeatherSummary>(summary))
                            .OrderBy(x=>x.Id)
                            .Skip(offset)
                            .Take(rows)
                            .Select(wf => new ForecastViewModel
                                          {
                                              CityName    = wf.City.Name,
                                              StateName   = wf.City.State.Name,
                                              CountryName = wf.City.State.Country.Name,
                                              Summary     = wf.Summary.ToString(),
                                              Date        = wf.Date
                                          }).ToListAsync();
    }
}