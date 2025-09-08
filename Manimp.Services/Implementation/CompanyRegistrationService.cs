using Microsoft.AspNetCore.Identity;
using Manimp.Auth.Models;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

public class CompanyRegistrationService : ICompanyRegistrationService
{
    private readonly ITenantService _tenantService;
    private readonly IAuthService _authService;

    public CompanyRegistrationService(ITenantService tenantService, IAuthService authService)
    {
        _tenantService = tenantService;
        _authService = authService;
    }

    public async Task<Guid> RegisterCompanyAsync(CompanyRegistrationRequest request)
    {
        // Create tenant
        var tenant = await _tenantService.CreateTenantAsync(request.CompanyName);

        try
        {
            // Create admin user in tenant database
            var createUserRequest = new CreateUserRequest
            {
                Email = request.AdminEmail,
                Password = request.AdminPassword,
                FirstName = "Admin",
                LastName = "User"
            };

            var userId = await _authService.CreateUserAsync(createUserRequest, tenant.TenantId);

            // Add user to directory
            await _tenantService.AddUserToDirectoryAsync(
                request.AdminEmail.ToUpperInvariant(), 
                tenant.TenantId);

            // TODO: Assign Admin role to the user
            // This would require role management implementation

            return tenant.TenantId;
        }
        catch
        {
            // TODO: Cleanup tenant if user creation fails
            // This would require implementing tenant deletion
            throw;
        }
    }
}