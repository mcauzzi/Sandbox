var builder = DistributedApplication.CreateBuilder(args);
var dataBase = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin(x=>x.WithLifetime(ContainerLifetime.Persistent))
                      .WithLifetime(ContainerLifetime.Persistent);
var weatherDb = dataBase.AddDatabase("weatherdb");
var migrationService=builder.AddProject<Projects.WeatherDbMigrationService>("migrations")
       .WithReference(weatherDb).WaitFor(weatherDb);
var cache = builder.AddRedis("cache").WithDataVolume(isReadOnly: false).WithRedisCommander();
builder.AddProject<Projects.WebApiSandbox>("WebApi")
       .WaitForCompletion(migrationService)
       .WaitFor(cache)
       .WithReference(weatherDb)
       .WithReference(cache);

builder.Build().Run();