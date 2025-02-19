using System.Diagnostics;
using AuthContextNs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;

namespace AuthDbMigration;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
    : BackgroundService
{
    public const            string         ActivitySourceName = "AuthDbMigrations";
    private static readonly ActivitySource ActivitySource     = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope       = serviceProvider.CreateScope();
            var       dbContext   = scope.ServiceProvider.GetRequiredService<AuthContext>();
            var       roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var       userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext,roleManager,userManager, cancellationToken);
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
    }

    private static async Task EnsureDatabaseAsync(AuthContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
                                    {
                                        using var activity =
                                            ActivitySource.StartActivity("Database Creation", ActivityKind.Client);
                                        // Create the database if it does not exist.
                                        // Do this first so there is then a database to start a transaction against.
                                        if (!await dbCreator.ExistsAsync(cancellationToken))
                                        {
                                            await dbCreator.CreateAsync(cancellationToken);
                                        }
                                    });
    }

    private static async Task RunMigrationAsync(AuthContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        using var activity = ActivitySource.StartActivity(ActivityKind.Client);
                                        // Run migration in a transaction to avoid partial migration if it fails.
                                        await dbContext.Database.MigrateAsync(cancellationToken);
                                    });
    }

    private async Task SeedDataAsync(AuthContext               dbContext,   RoleManager<IdentityRole> roleManager,
                                     UserManager<IdentityUser> userManager, CancellationToken         cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
                                    {
                                        using var activity = ActivitySource.StartActivity(ActivityKind.Client);
                                        // Seed the database
                                        await using var transaction =
                                            await dbContext.Database.BeginTransactionAsync(cancellationToken);
                                        var roles = new[] { "Admins", "Users" };

                                        foreach (var role in roles)
                                        {
                                            if (!await roleManager.RoleExistsAsync(role))
                                            {
                                                await roleManager.CreateAsync(new IdentityRole(role));
                                            }
                                        }

                                        var adminUser = new IdentityUser
                                                        { UserName = "Admin", Email = "admin@admins.com" };
                                        if (await userManager.FindByNameAsync(adminUser.UserName) == null)
                                        {
                                            var result = await userManager.CreateAsync(adminUser, "Admin123@");
                                            if (result.Succeeded)
                                            {
                                                await userManager.AddToRoleAsync(adminUser, "Users");
                                                await userManager.AddToRoleAsync(adminUser, "Admins");
                                            }
                                        }

                                        var basicUser = new IdentityUser
                                                        { UserName = "basicUser", Email = "basicUser@users.com" };
                                        if (await userManager.FindByNameAsync(basicUser.UserName) == null)
                                        {
                                            var result = await userManager.CreateAsync(basicUser, "User123@");
                                            if (result.Succeeded)
                                            {
                                                await userManager.AddToRoleAsync(basicUser, "Users");
                                            }
                                        }

                                        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                                        await transaction.CommitAsync(cancellationToken);
                                    });
    }
}