using EfCoreContext;
using EfCoreContext.Models;
using Microsoft.EntityFrameworkCore;
using RepositoriesExceptions;
using WebApiSandboxRepositoryInterfaces;
using WebApiSandboxViewModels;

namespace WebApiSandboxRepositories;

public class ForecastRepository(SandboxContext context) : IForecastsRepository
{
    private SandboxContext Context { get; } = context;

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
                          Temperature = wf.TemperatureC,
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
                                              Temperature = wf.TemperatureC,
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
                                              Temperature = wf.TemperatureC,
                                              Date        = wf.Date
                                          }).ToListAsync();
    }

    public async Task<IEnumerable<ForecastViewModel>> GetBySummary(string summary, int rows, int offset)
    {
        return await Context.WeatherForecasts
                            .Where(x=>x.Summary==Enum.Parse<WeatherWmoCode>(summary))
                            .OrderBy(x=>x.Id)
                            .Skip(offset)
                            .Take(rows)
                            .Select(wf => new ForecastViewModel
                                          {
                                              CityName    = wf.City.Name,
                                              StateName   = wf.City.State.Name,
                                              CountryName = wf.City.State.Country.Name,
                                              Summary     = wf.Summary.ToString(),
                                              Temperature = wf.TemperatureC,
                                              Date        = wf.Date
                                          }).ToListAsync();
    }
    
    public async Task Add(ForecastViewModel forecast)
    {
        var city = await Context.Cities
                               .Include(x=>x.State)
                               .ThenInclude(x=>x.Country)
                               .FirstOrDefaultAsync(x => x.Name == forecast.CityName);
        if (city == null)
        {
            throw new NotFoundException(nameof(City), forecast.CityName);
        }
        await Context.WeatherForecasts.AddAsync(new WeatherForecast
        {
            City    = city,
            Summary = Enum.Parse<WeatherWmoCode>(forecast.Summary),
            TemperatureC = forecast.Temperature,
            Date    = forecast.Date
        });
        await Context.SaveChangesAsync();
    }
}