using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Manimp.Auth.Models;
using Manimp.Data.Contexts;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    // private readonly SignInManager<ApplicationUser> _signInManager; // TODO: Implement when needed
    private readonly ITenantDbContext _tenantDbContext;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        // SignInManager<ApplicationUser> signInManager, // TODO: Implement when needed
        ITenantDbContext tenantDbContext)
    {
        _userManager = userManager;
        // _signInManager = signInManager; // TODO: Implement when needed
        _tenantDbContext = tenantDbContext;
    }

    public async Task<bool> ValidateUserAsync(string email, string password, Guid tenantId)
    {
        // Switch to tenant database context
        var connectionString = _tenantDbContext.GetConnectionString(tenantId);

        // Create tenant-specific DbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var tenantDb = new AppDbContext(options);

        var user = await tenantDb.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpperInvariant());
        if (user == null) return false;

        // Verify password
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);

        return result == PasswordVerificationResult.Success;
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