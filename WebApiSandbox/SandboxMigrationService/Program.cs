using EfCoreContext;
using SandboxAspireServiceDefaults;
using SandboxMigrationService;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
using var serilogTracing=builder.AddSerilogTracing();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));
builder.AddNpgsqlDbContext<SandboxContext>(connectionName: "WeatherDb");
var host = builder.Build();
host.Run();
