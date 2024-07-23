// See https://aka.ms/new-console-template for more information

using DbDataGenerator;
using EfCoreContext;
using EfCoreContext.Models;

const int NUM_CITIES_PER_STATE   = 10;
const int NUM_STATES_PER_COUNTRY = 10;
const int NUM_COUNTRIES          = 192;
const int NUM_FORECASTS_PER_CITY = 10;
var       countries              = new List<Country>();
countries.AddRange(Enumerable.Range(0, NUM_COUNTRIES)
                             .Select(i => new Country
                                          {
                                              Name = $"Country {i}",
                                              States = new(Enumerable.Range(0, NUM_STATES_PER_COUNTRY)
                                                                     .Select(j => new State
                                                                                 {
                                                                                     Name = $"State {j}",
                                                                                     Cities = Enumerable
                                                                                         .Range(0, NUM_CITIES_PER_STATE)
                                                                                         .Select(k => new City
                                                                                             {
                                                                                                 Name =
                                                                                                     $"City {k}",
                                                                                                 Country =
                                                                                                     $"Country {i}",
                                                                                                 State =
                                                                                                     $"State {j}",
                                                                                                 Description =
                                                                                                     $"City {k} in State {j} in Country {i}"
                                                                                             })
                                                                                         .ToHashSet()
                                                                                 }))
                                          }));
var context = new SandboxContext();
foreach (var country in countries)
{
    foreach (var state in country.States)
    {
        foreach (var city in state.Cities)
        {
            var cityForecasts = Enumerable.Range(0, NUM_FORECASTS_PER_CITY)
                                          .Select(_ => new WeatherForecast
                                                       {
                                                           CityId       = city.Id,
                                                           Date         = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(Random.Shared.Next(-365, 365)),
                                                           Summary      = "Sunny",
                                                           TemperatureC = Random.Shared.Next(-20, 55)
                                                       });
            foreach (var forecast in cityForecasts)
            {
                city.WeatherForecasts.Add(forecast);
            }
            context.Cities.Add(city);
        }

        await context.SaveChangesAsync();
    }
}