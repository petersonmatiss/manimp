using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Manimp.Data.Contexts;

namespace Manimp.Data;

/// <summary>
/// Design-time factory for creating AppDbContext instances during EF Core migrations
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new instance of AppDbContext for design-time operations
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>A configured AppDbContext instance</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Try to get connection string from configuration first
        var connectionString = GetConnectionString();

        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Gets the connection string for design-time operations
    /// </summary>
    /// <returns>The connection string to use</returns>
    private static string GetConnectionString()
    {
        // Try to read from configuration if available
        var configuration = TryBuildConfiguration();
        var connectionString = configuration?.GetConnectionString("TenantTemplate");

        // Fall back to default connection string for migrations
        return connectionString?.Replace("{DB}", "ManimpTenant")
               ?? "Server=localhost;Database=ManimpTenant;Trusted_Connection=true;TrustServerCertificate=true;";
    }

    /// <summary>
    /// Attempts to build configuration from appsettings files
    /// </summary>
    /// <returns>Configuration instance or null if not available</returns>
    private static IConfiguration? TryBuildConfiguration()
    {
        try
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddUserSecrets<AppDbContextFactory>(optional: true)
                .Build();
        }
        catch
        {
            // If configuration building fails, return null to use fallback
            return null;
        }
    }
}