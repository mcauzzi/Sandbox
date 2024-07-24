// See https://aka.ms/new-console-template for more information

using EfCoreContext;
using EfCoreContext.Models;

const int NUM_CITIES_PER_STATE   = 10;
const int NUM_STATES_PER_COUNTRY = 10;
const int NUM_COUNTRIES          = 192;
const int NUM_FORECASTS_PER_CITY = 10;
var       weatherSummaryValues   = Enum.GetValues<WeatherSummary>();
var       countries              = new List<Country>();
var dates = Enumerable.Range(0, NUM_FORECASTS_PER_CITY)
                            .Select(i => DateOnly.FromDateTime(DateTime.UtcNow).AddDays(i))
                            .ToArray();
countries.AddRange(Enumerable.Range(0, NUM_COUNTRIES)
                             .Select(i => new Country
                                          {
                                              Name = $"Country {i}",
                                              States = new(Enumerable.Range(0, NUM_STATES_PER_COUNTRY)
                                                                     .Select(j => new State
                                                                                 {
                                                                                     Name = $"State {j}",
                                                                                 }))
                                          }));

var context = new SandboxContext();
context.Countries.AddRange(countries);
foreach (var state in countries.SelectMany(x=>x.States))
{
    state.Cities = new(Enumerable.Range(0, NUM_CITIES_PER_STATE)
                                 .Select(i => new City
                                              {
                                                  Name = $"City {i}",
                                                  StateId = state.Id,
                                                  WeatherForecasts = new()
                                              }));
    await context.SaveChangesAsync();
}
foreach (var country in countries)
{
    foreach (var state in country.States)
    {
        foreach (var city in state.Cities)
        {
            var cityForecasts = dates
                                          .Select(date => new WeatherForecast
                                                       {
                                                           CityId = city.Id,
                                                           Date = date,
                                                           Summary =
                                                               weatherSummaryValues
                                                                   [Random.Shared.Next(0, weatherSummaryValues.Length)],
                                                           TemperatureC = Random.Shared.Next(-20, 55)
                                                       });
            context.WeatherForecasts.AddRange(cityForecasts);
        }
    }
    await context.SaveChangesAsync();
}