using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Manimp.Directory.Data;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

public class TenantService : ITenantService
{
    private readonly DirectoryDbContext _directoryDb;
    private readonly IConfiguration _configuration;

    public TenantService(DirectoryDbContext directoryDb, IConfiguration configuration)
    {
        _directoryDb = directoryDb;
        _configuration = configuration;
    }

    public async Task<Tenant> CreateTenantAsync(string companyName)
    {
        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            Name = companyName,
            DbName = $"manimp_{Ulid.NewUlid().ToString().ToLowerInvariant()}",
            SecretRef = Guid.NewGuid().ToString(), // This would be a reference to a secure secret store
            IsActive = true,
            CreatedUtc = DateTime.UtcNow
        };

        _directoryDb.Tenants.Add(tenant);
        await _directoryDb.SaveChangesAsync();

        // Create the actual database
        await CreateTenantDatabaseAsync(tenant);

        return tenant;
    }

    public async Task<Tenant?> GetTenantAsync(Guid tenantId)
    {
        return await _directoryDb.Tenants
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.IsActive);
    }

    public async Task<List<Guid>> GetTenantIdsByEmailAsync(string normalizedEmail)
    {
        return await _directoryDb.UserDirectory
            .Where(ud => ud.NormalizedEmail == normalizedEmail)
            .Select(ud => ud.TenantId)
            .ToListAsync();
    }

    public async Task AddUserToDirectoryAsync(string normalizedEmail, Guid tenantId)
    {
        var existingEntry = await _directoryDb.UserDirectory
            .FirstOrDefaultAsync(ud => ud.NormalizedEmail == normalizedEmail && ud.TenantId == tenantId);

        if (existingEntry == null)
        {
            _directoryDb.UserDirectory.Add(new UserDirectory
            {
                NormalizedEmail = normalizedEmail,
                TenantId = tenantId
            });
            await _directoryDb.SaveChangesAsync();
        }
    }

    private async Task CreateTenantDatabaseAsync(Tenant tenant)
    {
        var adminConnectionString = _configuration.GetConnectionString("SqlServerAdmin");
        var tenantTemplate = _configuration.GetConnectionString("TenantTemplate");

        if (string.IsNullOrEmpty(adminConnectionString) || string.IsNullOrEmpty(tenantTemplate))
        {
            throw new InvalidOperationException("Database connection strings not configured");
        }

        // Create database using admin connection
        using var adminConnection = new Microsoft.Data.SqlClient.SqlConnection(adminConnectionString);
        await adminConnection.OpenAsync();

        var createDbCommand = new Microsoft.Data.SqlClient.SqlCommand(
            $"CREATE DATABASE [{tenant.DbName}]", adminConnection);
        await createDbCommand.ExecuteNonQueryAsync();

        // Run migrations on the new tenant database
        var tenantConnectionString = tenantTemplate.Replace("{DB}", tenant.DbName);

        // Run EF migrations on the new tenant database
        await RunTenantMigrationsAsync(tenantConnectionString);
    }

    private async Task RunTenantMigrationsAsync(string connectionString)
    {
        // This will use EF Core migrations to create the tenant database schema
        // Implementation will be added when tenant migration logic is implemented
        await Task.CompletedTask;
    }
}
