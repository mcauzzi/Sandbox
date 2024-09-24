using System.Text;
using EfCoreContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SandboxAuthentication;
using SandboxAuthenticationInterfaces;
using SandboxConfigurations;
using WebApiSandboxControllers;
using WebApiSandboxRepositories;
using WebApiSandboxRepositoryInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection(nameof(AuthConfig)));
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
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddEntityFrameworkStores<SandboxContext>()
       .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
                                   {
                                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                       options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                                   })
       .AddJwtBearer(options =>
                     {
                         options.TokenValidationParameters = new TokenValidationParameters
                                                             {
                                                                 ValidateIssuer = true,
                                                                 ValidateAudience = true,
                                                                 ValidateLifetime = true,
                                                                 ValidateIssuerSigningKey = true,
                                                                 ValidIssuer = "Sandbox",
                                                                 ValidAudience = "Audience",
                                                                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey"))
                                                             };
                     });
builder.Services.AddScoped<ISecretsProvider, SecretsProvider>();
builder.Services.AddScoped<ITokenService, TokenService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();