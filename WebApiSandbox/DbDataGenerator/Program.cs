// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.Json;
using EfCoreContext;
using EfCoreContext.Models;

const int NUM_CITIES_PER_STATE   = 10;
const int NUM_STATES_PER_COUNTRY = 10;
const int NUM_COUNTRIES          = 192;
const int NUM_FORECASTS_PER_CITY = 10;

var dates = Enumerable.Range(0, NUM_FORECASTS_PER_CITY)
                      .Select(i => DateOnly.FromDateTime(DateTime.UtcNow).AddDays(i))
                      .ToArray();
var countries = JsonSerializer.Deserialize<List<JsonCountry>>(await File.ReadAllTextAsync("./Data/countries.json"));
var states    = JsonSerializer.Deserialize<List<JsonState>>(await File.ReadAllTextAsync("./Data/states.json"));
var cities    = JsonSerializer.Deserialize<List<JsonCity>>(await File.ReadAllTextAsync("./Data/cities.json"));
var context   = new SandboxContext();

await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
context.Countries.AddRangeAsync(countries.Select(x => new Country()
                                                      {
                                                          Name = x.name,
                                                          Id   = x.id,
                                                      }));
await context.SaveChangesAsync();
await context.States.AddRangeAsync(states.Select(x => new State()
                                                      {
                                                          CountryId = x.country_id, Id = x.id, Name = x.name
                                                      }));
await context.SaveChangesAsync();
await context.Cities.AddRangeAsync(cities.Select(x => new City()
                                                      {
                                                          StateId   = x.state_id, Id = x.id, Name = x.name,
                                                          Latitude  = Convert.ToDecimal(x.latitude, CultureInfo.InvariantCulture),
                                                          Longitude = Convert.ToDecimal(x.longitude,CultureInfo.InvariantCulture)
                                                      }));
await context.SaveChangesAsync();