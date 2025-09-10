using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Manimp.Auth.Models;
using Manimp.Data.Contexts;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantDbContext _tenantDbContext;

    public AuthService(
        IServiceProvider serviceProvider,
        ITenantDbContext tenantDbContext)
    {
        _serviceProvider = serviceProvider;
        _tenantDbContext = tenantDbContext;
    }

    public async Task<bool> ValidateUserAsync(string email, string password, Guid tenantId)
    {
        var connectionString = _tenantDbContext.GetConnectionString(tenantId);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var tenantDb = new AppDbContext(options);

        using var scope = _serviceProvider.CreateScope();

        var userStore = new UserStore<ApplicationUser>(tenantDb);
        var userManager = ActivatorUtilities.CreateInstance<UserManager<ApplicationUser>>(scope.ServiceProvider, userStore);
        var signInManager = ActivatorUtilities.CreateInstance<SignInManager<ApplicationUser>>(scope.ServiceProvider, userManager);

        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        return result.Succeeded;
    }

    public async Task<string> CreateUserAsync(CreateUserRequest request, Guid tenantId)
    {
        var connectionString = _tenantDbContext.GetConnectionString(tenantId);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var tenantDb = new AppDbContext(options);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpperInvariant(),
            NormalizedUserName = request.Email.ToUpperInvariant(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true, // For simplicity in Phase 1
            CreatedUtc = DateTime.UtcNow
        };

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        tenantDb.Users.Add(user);
        await tenantDb.SaveChangesAsync();

        return user.Id;
    }
}