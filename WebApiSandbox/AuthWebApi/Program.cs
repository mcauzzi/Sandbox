using AuthContextNs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SandboxAspireServiceDefaults;
using SandboxAuthentication;
using SandboxAuthenticationInterfaces;
using SandboxConfigurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection(nameof(AuthConfig)));
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddNpgsqlDbContext<AuthContext>(connectionName: "sandboxAuthDb");
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddRoles<IdentityRole>()
       .AddEntityFrameworkStores<AuthContext>()
       .AddDefaultTokenProviders();
builder.AddServiceDefaults();
builder.Services.AddScoped<ISecretsProvider, SecretsProvider>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(options =>
                                   {
                                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                       options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                                   })
       .AddJwtBearer(options =>
                     {
                         options.IncludeErrorDetails = true;
                         options.TokenValidationParameters = new TokenValidationParameters
                                                             {
                                                                 ValidateIssuer           = true,
                                                                 ValidateAudience         = true,
                                                                 ValidateLifetime         = true,
                                                                 ValidateIssuerSigningKey = true,
                                                                 ValidIssuer              = "SandboxApi",
                                                                 ValidAudience            = "SandboxClient",
                                                                 IssuerSigningKey =
                                                                     new SymmetricSecurityKey("zC8vVKxMAraTYlxRI3tXVi17lWv24UZLD081L7hdObY="u8.ToArray())
                                                             };
                     });
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
var summaries = new[]
                {
                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering",
                    "Scorching"
                };

app.MapGet("/weatherforecast", () =>
                               {
                                   var forecast = Enumerable.Range(1, 5)
                                                            .Select(index =>
                                                                        new
                                                                            WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                                             Random.Shared.Next(-20, 55),
                                                                             summaries
                                                                                 [Random.Shared.Next(summaries.Length)]))
                                                            .ToArray();
                                   return forecast;
                               })
   .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}