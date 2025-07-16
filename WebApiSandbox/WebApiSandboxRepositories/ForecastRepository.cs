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
                          Latitude   = wf.Latitude,
                          Longitude  = wf.Longitude,
                          Summary     = wf.Summary.ToString(),
                          Temperature = wf.TemperatureC,
                          Date        = wf.Date
                      }).ToListAsync();
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
                                              Summary     = wf.Summary.ToString(),
                                              Temperature = wf.TemperatureC,
                                              Date        = wf.Date
                                          }).ToListAsync();
    }
    
    public async Task Add(ForecastViewModel forecast)
    {
        await Context.WeatherForecasts.AddAsync(new WeatherData
        {
            Latitude = forecast.Latitude,
            Longitude = forecast.Longitude,
            Summary = Enum.Parse<WeatherWmoCode>(forecast.Summary),
            TemperatureC = forecast.Temperature,
            Date    = forecast.Date
        });
        await Context.SaveChangesAsync();
    }
}