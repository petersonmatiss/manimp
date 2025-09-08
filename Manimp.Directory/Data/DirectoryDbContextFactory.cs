using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Manimp.Directory.Data;

public class DirectoryDbContextFactory : IDesignTimeDbContextFactory<DirectoryDbContext>
{
    public DirectoryDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("Directory") ??
            "Server=(localdb)\\mssqllocaldb;Database=ManimpDirectory;Trusted_Connection=true;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<DirectoryDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DirectoryDbContext(optionsBuilder.Options);
    }
}