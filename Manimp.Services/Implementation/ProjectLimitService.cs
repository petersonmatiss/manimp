using Microsoft.EntityFrameworkCore;
using Manimp.Directory.Data;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

/// <summary>
/// Service for managing tenant project limits and EN 1090 compliance
/// </summary>
public class ProjectLimitService : IProjectLimitService
{
    private readonly DirectoryDbContext _directoryContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectLimitService"/> class
    /// </summary>
    /// <param name="directoryContext">The directory database context</param>
    public ProjectLimitService(DirectoryDbContext directoryContext)
    {
        _directoryContext = directoryContext;
    }

    /// <summary>
    /// Checks if a tenant can create a new project this month
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>True if the tenant can create a project, false otherwise</returns>
    public async Task<bool> CanCreateProjectAsync(Guid tenantId)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
        var limits = await GetOrCreateProjectLimitAsync(tenantId, currentMonth);
        
        return !limits.HasReachedLimit;
    }

    /// <summary>
    /// Gets the remaining project slots for the current month
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The number of remaining project slots</returns>
    public async Task<int> GetRemainingProjectsAsync(Guid tenantId)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
        var limits = await GetOrCreateProjectLimitAsync(tenantId, currentMonth);
        
        return limits.RemainingProjects;
    }

    /// <summary>
    /// Increments the project count for a tenant (call when a project is created)
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>True if the project was counted successfully, false if limit exceeded</returns>
    public async Task<bool> IncrementProjectCountAsync(Guid tenantId)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
        var limits = await GetOrCreateProjectLimitAsync(tenantId, currentMonth);
        
        if (limits.HasReachedLimit)
        {
            return false;
        }

        limits.ProjectsCreated++;
        limits.UpdatedUtc = DateTime.UtcNow;
        
        await _directoryContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Decrements the project count for a tenant (call when a project is deleted)
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>True if the project was decremented successfully</returns>
    public async Task<bool> DecrementProjectCountAsync(Guid tenantId)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
        var limits = await GetOrCreateProjectLimitAsync(tenantId, currentMonth);
        
        if (limits.ProjectsCreated > 0)
        {
            limits.ProjectsCreated--;
            limits.UpdatedUtc = DateTime.UtcNow;
            
            await _directoryContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds addon projects to a tenant's monthly limit
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="additionalProjects">Number of additional projects to add</param>
    /// <returns>The updated project limit record</returns>
    public async Task<TenantProjectLimit> AddAddonProjectsAsync(Guid tenantId, int additionalProjects)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
        var limits = await GetOrCreateProjectLimitAsync(tenantId, currentMonth);
        
        limits.AddonProjects += additionalProjects;
        limits.UpdatedUtc = DateTime.UtcNow;
        
        await _directoryContext.SaveChangesAsync();
        return limits;
    }

    /// <summary>
    /// Gets the project limits for a specific tenant and month
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="month">The month in yyyy-MM format</param>
    /// <returns>The project limit record</returns>
    public async Task<TenantProjectLimit> GetProjectLimitAsync(Guid tenantId, string month)
    {
        return await GetOrCreateProjectLimitAsync(tenantId, month);
    }

    /// <summary>
    /// Gets or creates a project limit record for a tenant and month
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="month">The month in yyyy-MM format</param>
    /// <returns>The project limit record</returns>
    private async Task<TenantProjectLimit> GetOrCreateProjectLimitAsync(Guid tenantId, string month)
    {
        var limits = await _directoryContext.TenantProjectLimits
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Month == month);

        if (limits == null)
        {
            limits = new TenantProjectLimit
            {
                TenantId = tenantId,
                Month = month,
                ProjectsCreated = 0,
                BaseLimit = 10,
                AddonProjects = 0,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };

            _directoryContext.TenantProjectLimits.Add(limits);
            await _directoryContext.SaveChangesAsync();
        }

        return limits;
    }

    /// <summary>
    /// Gets project limit history for a tenant
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="monthsBack">Number of months back to retrieve (default: 12)</param>
    /// <returns>List of project limits</returns>
    public async Task<List<TenantProjectLimit>> GetProjectLimitHistoryAsync(Guid tenantId, int monthsBack = 12)
    {
        var startDate = DateTime.UtcNow.AddMonths(-monthsBack);
        var startMonth = startDate.ToString("yyyy-MM");

        return await _directoryContext.TenantProjectLimits
            .Where(x => x.TenantId == tenantId && string.Compare(x.Month, startMonth) >= 0)
            .OrderByDescending(x => x.Month)
            .ToListAsync();
    }
}

