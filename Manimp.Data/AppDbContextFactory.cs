using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Manimp.Data.Contexts;

namespace Manimp.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use a placeholder connection string for migrations
        // This will be replaced at runtime with tenant-specific connection strings
        optionsBuilder.UseSqlServer("Server=localhost;Database=ManimpTenant;Trusted_Connection=true;TrustServerCertificate=true;");

        return new AppDbContext(optionsBuilder.Options);
    }
}