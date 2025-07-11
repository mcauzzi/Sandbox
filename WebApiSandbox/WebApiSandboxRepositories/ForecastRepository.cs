using System.Text.Json;
using EfCoreContext;
using Microsoft.EntityFrameworkCore;
using Models;
using RepositoriesExceptions;
using StackExchange.Redis;
using WebApiSandboxRepositoryInterfaces;
using WebApiSandboxViewModels;

namespace WebApiSandboxRepositories;

public class ForecastRepository(SandboxContext context,IConnectionMultiplexer conn) : IForecastsRepository
{
    private SandboxContext Context { get; } = context;
    private IDatabase      Redis   { get; }= conn.GetDatabase();

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
        var cachedVal = await Redis.StringGetAsync($"{cityId}|{rows}|{offset}");
        if(cachedVal.HasValue)
        {
           return JsonSerializer.Deserialize<IEnumerable<ForecastViewModel>>(cachedVal);
        }

        var dbRes= await Context.WeatherForecasts
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
        await Redis.StringSetAsync($"{cityId}|{rows}|{offset}",JsonSerializer.Serialize(dbRes),TimeSpan.FromMinutes(5));
        return dbRes;

    }

    public async Task<IEnumerable<ForecastViewModel>> GetByDate(DateTime date, int rows, int offset)
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