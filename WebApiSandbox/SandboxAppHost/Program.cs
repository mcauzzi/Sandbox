var builder = DistributedApplication.CreateBuilder(args);
var dataBase = builder.AddPostgres("postgres", port: 5432)
                      .WithDataVolume()
                      .WithPgAdmin(x => x.WithLifetime(ContainerLifetime.Persistent).WithHostPort(5433))
                      .WithLifetime(ContainerLifetime.Persistent);
var weatherDb = dataBase.AddDatabase("weatherdb");
var migrationService = builder.AddProject<Projects.WeatherDbMigrationService>("migrations")
                              .WithReference(weatherDb)
                              .WaitFor(weatherDb);
var cache = builder.AddRedis("cache", port: 5434)
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithDataVolume(isReadOnly: false)
                   .WithRedisCommander(x => x.WithHostPort(5435).WithLifetime(ContainerLifetime.Persistent));
builder.AddProject<Projects.WebApiSandbox>("WebApi")
       .WaitForCompletion(migrationService)
       .WaitFor(cache)
       .WithReference(weatherDb)
       .WithReference(cache);

builder.Build().Run();