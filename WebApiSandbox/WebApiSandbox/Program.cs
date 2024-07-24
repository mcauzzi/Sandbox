using EfCoreContext;
using Microsoft.EntityFrameworkCore;
using WebApiSandboxControllers;
using WebApiSandboxRepositories;
using WebApiSandboxRepositoryInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IForecastsRepository, ForecastRepository>();
builder.Services.AddControllers()
       .AddApplicationPart(typeof(WeatherForecastController).Assembly)
       .AddControllersAsServices();
builder.Services.AddDbContext<SandboxContext>(x =>
                                              {
                                                  x.EnableSensitiveDataLogging();
                                                  x.UseNpgsql(builder.Configuration
                                                                     .GetConnectionString("DefaultConnection"));
                                              });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();