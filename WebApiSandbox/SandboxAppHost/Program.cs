var builder = DistributedApplication.CreateBuilder(args);
var dataBase = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin()
                      .WithLifetime(ContainerLifetime.Persistent);
var weatherDb = dataBase.AddDatabase("weatherdb");
var migrationService=builder.AddProject<Projects.WeatherDbMigrationService>("migrations")
       .WithReference(weatherDb).WaitFor(weatherDb);
builder.AddProject<Projects.WebApiSandbox>("WebApi").WaitForCompletion(migrationService).WithReference(weatherDb);

builder.Build().Run();