using System.Text;
using EfCoreContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SandboxAspireServiceDefaults;
using SandboxAuthentication;
using SandboxAuthenticationInterfaces;
using SandboxConfigurations;
using SandboxRemoteApisImportersInterfaces;
using SandboxRemoteApisImporters;
using SandboxServices;
using WebApiSandboxControllers;
using WebApiSandboxRepositories;
using WebApiSandboxRepositoryInterfaces;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection(nameof(AuthConfig)));
builder.Services.Configure<OpenMeteoImporterConfig>(builder.Configuration.GetSection(nameof(OpenMeteoImporterConfig)));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
                               {
                                   c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                                   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                                                     {
                                                                         In = ParameterLocation.Header,
                                                                         Description =
                                                                             "Please enter into field the word 'Bearer' followed by a space and the JWT value",
                                                                         Name   = "Authorization",
                                                                         Type   = SecuritySchemeType.ApiKey,
                                                                         Scheme = "Bearer"
                                                                     });
                                   c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                            {
                                                                {
                                                                    new OpenApiSecurityScheme
                                                                    {
                                                                        Reference = new OpenApiReference
                                                                            {
                                                                                Type = ReferenceType
                                                                                    .SecurityScheme,
                                                                                Id = "Bearer"
                                                                            }
                                                                    },
                                                                    new string[] { }
                                                                }
                                                            });
                               });
builder.Services.AddScoped<IForecastsRepository, ForecastRepository>();
builder.Services.AddControllers()
       .AddApplicationPart(typeof(WeatherForecastController).Assembly)
       .AddControllersAsServices();
builder.AddServiceDefaults();
builder.AddRedisClient(connectionName: "cache");
builder.AddNpgsqlDbContext<SandboxContext>(connectionName: "WeatherDb");
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddRoles<IdentityRole>()
       .AddEntityFrameworkStores<SandboxContext>()
       .AddDefaultTokenProviders();
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
                                                                     new SymmetricSecurityKey(Encoding.UTF8
                                                                         .GetBytes("zC8vVKxMAraTYlxRI3tXVi17lWv24UZLD081L7hdObY="))
                                                             };
                     });
builder.Services.AddScoped<ISecretsProvider, SecretsProvider>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IWeatherImport, OpenMeteoImporter>();
builder.Services.AddHostedService<WeatherDataImporter>();
builder.Services.AddHttpClient<IWeatherImport, OpenMeteoImporter>();
// builder.Services.AddAuthorizationBuilder()
//        .AddPolicy("Users",  policy => policy.RequireRole("Users"))
//        .AddPolicy("Admins", policy => policy.RequireRole("Admins"));
var app = builder.Build();
// Create roles if they don't exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await AuthInitializer.Initialize(roleManager, userManager);
}

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