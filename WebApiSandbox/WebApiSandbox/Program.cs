using EfCoreContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SandboxAspireServiceDefaults;
using SandboxConfigurations;
using SandboxRemoteApisImportersInterfaces;
using SandboxRemoteApisImporters;
using SandboxServices;
using WeatherImportConfigs;
using WebApiSandboxControllers;
using WebApiSandboxRepositories;
using WebApiSandboxRepositoryInterfaces;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddWeatherImporterServices(builder.Configuration.GetSection(nameof(OpenMeteoImporterConfig)));
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
                                   c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                                                                   {
                                                                       {
                                                                           new
                                                                               OpenApiSecuritySchemeReference("Bearer",
                                                                                doc),
                                                                           []
                                                                       }
                                                                   });
                               });
builder.Services.AddScoped<IForecastsRepository, ForecastRepository>();
builder.Services.AddControllers()
       .AddApplicationPart(typeof(WeatherForecastController).Assembly)
       .AddControllersAsServices();
builder.AddServiceDefaults();
builder.AddRedisClient(connectionName: "cache");
builder.AddNpgsqlDbContext<SandboxContext>(connectionName: "WeatherDb", options =>
                                                                        {
                                                                            options.DisableMetrics = false;
                                                                            options.DisableTracing = false;
                                                                        });

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
                                                                     new
                                                                         SymmetricSecurityKey("zC8vVKxMAraTYlxRI3tXVi17lWv24UZLD081L7hdObY="u8
                                                                             .ToArray())
                                                             };
                     });


// builder.Services.AddAuthorizationBuilder()
//        .AddPolicy("Users",  policy => policy.RequireRole("Users"))
//        .AddPolicy("Admins", policy => policy.RequireRole("Admins"));
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