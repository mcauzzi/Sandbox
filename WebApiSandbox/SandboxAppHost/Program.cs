var builder = DistributedApplication.CreateBuilder(args);
var dataBase = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin()
                      .WithLifetime(ContainerLifetime.Persistent);
var weatherDb = dataBase.AddDatabase("weatherdb");
builder.AddProject<Projects.WebApiSandbox>("WebApi").WaitFor(weatherDb).WithReference(weatherDb);
builder.Build().Run();