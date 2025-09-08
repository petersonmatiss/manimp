using Manimp.Shared.Models;

namespace Manimp.Shared.Interfaces;

public interface ITenantService
{
    Task<Tenant> CreateTenantAsync(string companyName);
    Task<Tenant?> GetTenantAsync(Guid tenantId);
    Task<List<Guid>> GetTenantIdsByEmailAsync(string normalizedEmail);
    Task AddUserToDirectoryAsync(string normalizedEmail, Guid tenantId);
}

public interface ICompanyRegistrationService
{
    Task<Guid> RegisterCompanyAsync(CompanyRegistrationRequest request);
}

public interface IAuthService
{
    Task<bool> ValidateUserAsync(string email, string password, Guid tenantId);
    Task<string> CreateUserAsync(CreateUserRequest request, Guid tenantId);
}

public interface ITenantDbContext
{
    string GetConnectionString(Guid tenantId);
}