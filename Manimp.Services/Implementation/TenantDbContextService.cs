using Microsoft.Extensions.Configuration;
using Manimp.Shared.Interfaces;

namespace Manimp.Services.Implementation;

public class TenantDbContextService : ITenantDbContext
{
    private readonly IConfiguration _configuration;

    public TenantDbContextService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString(Guid tenantId)
    {
        // In a real implementation, you would:
        // 1. Look up the tenant in the directory database
        // 2. Get the database name and connection info
        // 3. Retrieve secrets from a secure store (Azure Key Vault, etc.)

        var tenantTemplate = _configuration.GetConnectionString("TenantTemplate");
        if (string.IsNullOrEmpty(tenantTemplate))
        {
            throw new InvalidOperationException("TenantTemplate connection string not configured");
        }

        // For now, we'll use a simple pattern - in production this would be more secure
        var dbName = $"manimp_{tenantId:N}".ToLowerInvariant();
        return tenantTemplate.Replace("{DB}", dbName);
    }
}