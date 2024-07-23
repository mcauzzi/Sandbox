using EfCoreContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SandboxContext>(x =>
                                              {
                                                  x.EnableSensitiveDataLogging();
                                                  x.UseNpgsql("Host=localhost;Database=sandbox;Username=sandbox;Password=sandbox123");
                                              });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/weatherforecast", (SandboxContext context) =>
                               {
                                   var forecast = context.WeatherForecasts.OrderBy(x => Guid.NewGuid())
                                                         .Take(10)
                                                         .ToList();
                                   return forecast;
                               })
   .WithName("GetWeatherForecast")
   .WithOpenApi();

app.Run();