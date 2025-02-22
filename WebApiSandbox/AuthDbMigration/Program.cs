using AuthContextEfCore;
using AuthDbMigration;
using Microsoft.AspNetCore.Identity;
using SandboxAspireServiceDefaults;


var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AuthContext>(connectionName: "sandboxAuthDb");
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddRoles<IdentityRole>()
       .AddEntityFrameworkStores<AuthContext>()
       .AddDefaultTokenProviders();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));
var host = builder.Build();
host.Run();
