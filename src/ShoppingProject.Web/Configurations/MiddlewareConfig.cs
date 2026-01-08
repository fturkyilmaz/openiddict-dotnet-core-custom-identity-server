using Ardalis.ListStartupServices;
using ShoppingProject.Infrastructure.Data;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace ShoppingProject.Web.Configurations;

public static class MiddlewareConfig
{
    public static async Task<WebApplication> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices

            // Scalar UI
            app.MapScalarApiReference();
        }
        else
        {
            // Global exception handler
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection(); // Note this will drop Authorization headers

        // Run migrations and seed in Development or when explicitly requested via environment variable
        var shouldMigrate = app.Environment.IsDevelopment() ||
                            app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

        if (shouldMigrate)
        {
            await MigrateDatabaseAsync(app);
            await SeedDatabaseAsync(app);
        }

        return app;
    }

    static async Task MigrateDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred migrating the DB. {exceptionMessage}", ex.Message);
            throw; // Re-throw to make startup fail if migrations fail
        }
    }

    static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Seeding database...");
            var context = services.GetRequiredService<AppDbContext>();
            await SeedData.InitializeAsync(context);
            logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
            // Don't re-throw for seeding errors - it's not critical
        }
    }
}
