var builder = DistributedApplication.CreateBuilder(args);
var dataBase = builder.AddPostgres("postgres", port: 5432)
                      .WithDataVolume()
                      .WithPgAdmin(x => x.WithLifetime(ContainerLifetime.Persistent)
                                         .WithHostPort(5433))
                      .WithLifetime(ContainerLifetime.Persistent);
var weatherDb = dataBase.AddDatabase("weatherdb");
var authDb=dataBase.AddDatabase("sandboxAuthDb");

var weatherDbMigrationService = builder.AddProject<Projects.WeatherDbMigrationService>("weatherMigrations")
                              .WithReference(weatherDb)
                              .WaitFor(weatherDb);
var authDbMigrationService = builder.AddProject<Projects.AuthDbMigration>("authDbMigrationService")
                                       .WithReference(authDb)
                                       .WaitFor(authDb);

var cache = builder.AddRedis("cache", port: 5434)
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithDataVolume(isReadOnly: false)
                   .WithRedisCommander(x => x.WithHostPort(5435)
                                           
                                             .WithLifetime(ContainerLifetime.Persistent));

builder.AddProject<Projects.WebApiSandbox>("WebApi")
       .WaitForCompletion(weatherDbMigrationService)
       .WaitForCompletion(authDbMigrationService)
       .WaitFor(cache)
       .WithReference(authDb)
       .WithReference(weatherDb)
       .WithReference(cache);

builder.Build().Run();