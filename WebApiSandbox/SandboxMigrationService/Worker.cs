using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using EfCoreContext;
using EfCoreContext.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using SandboxMigrationService.JsonModels;

namespace SandboxMigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
    : BackgroundService
{
    public const            string         ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource   = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope     = serviceProvider.CreateScope();
            var       dbContext = scope.ServiceProvider.GetRequiredService<SandboxContext>();

            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(SandboxContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
                                    {
                                        using var activity =
                                            s_activitySource.StartActivity("Database Creation", ActivityKind.Client);
                                        // Create the database if it does not exist.
                                        // Do this first so there is then a database to start a transaction against.
                                        if (!await dbCreator.ExistsAsync(cancellationToken))
                                        {
                                            await dbCreator.CreateAsync(cancellationToken);
                                        }
                                    });
    }

    private static async Task RunMigrationAsync(SandboxContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        using var activity =
                                            s_activitySource.StartActivity("Applying Migrations", ActivityKind.Client);
                                        // Run migration in a transaction to avoid partial migration if it fails.
                                        await dbContext.Database.MigrateAsync(cancellationToken);
                                    });
    }

    private static async Task SeedDataAsync(SandboxContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        using var activity =
                                            s_activitySource.StartActivity("Seeding Data", ActivityKind.Client);
                                        // Seed the database
                                        await using var transaction =
                                            await dbContext.Database.BeginTransactionAsync(cancellationToken);

                                        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                                        await AddCountries(dbContext, cancellationToken);
                                        await AddStates(dbContext, cancellationToken);
                                        await AddCities(dbContext, cancellationToken);
                                        await transaction.CommitAsync(cancellationToken);
                                    });
    }

    private static async Task AddStates(SandboxContext dbContext, CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Adding states", ActivityKind.Client);
        if (await dbContext.States.AnyAsync(cancellationToken: cancellationToken))
        {
            return;
        }

        var states =
            JsonSerializer.Deserialize<List<JsonState>>(await File.ReadAllTextAsync("./Data/states.json",
                                                         cancellationToken));
        await dbContext.States.AddRangeAsync(states.Select(x => new State()
                                                                {
                                                                    CountryId = x.country_id,
                                                                    Id        = x.id, Name = x.name
                                                                }), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task AddCities(SandboxContext dbContext, CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Adding cities", ActivityKind.Client);
        if (await dbContext.Cities.AnyAsync(cancellationToken: cancellationToken))
        {
            return;
        }

        var cities =
            JsonSerializer.Deserialize<List<JsonCity>>(await File.ReadAllTextAsync("./Data/cities.json",
                                                        cancellationToken));
        await dbContext.Cities.AddRangeAsync(cities.Select(x => new City()
                                                                {
                                                                    StateId = x.state_id,
                                                                    Id      = x.id,
                                                                    Name    = x.name,
                                                                    Latitude = Convert.ToDecimal(x.latitude,
                                                                     CultureInfo.InvariantCulture),
                                                                    Longitude =
                                                                        Convert.ToDecimal(x.longitude,
                                                                         CultureInfo.InvariantCulture)
                                                                }), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task AddCountries(SandboxContext dbContext, CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Adding countries", ActivityKind.Client);
        if (await dbContext.Countries.AnyAsync(cancellationToken: cancellationToken))
        {
            return;
        }

        var countries =
            JsonSerializer.Deserialize<List<JsonCountry>>(await File.ReadAllTextAsync("./Data/countries.json",
                                                           cancellationToken));
        await dbContext.Countries.AddRangeAsync(countries.Select(x => new Country()
                                                                      {
                                                                          Name = x.name,
                                                                          Id   = x.id,
                                                                      }), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}