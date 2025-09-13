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

/// <summary>
/// Service for EN 1090 compliance validation and project tier management
/// </summary>
public class EN1090ComplianceService : IEN1090ComplianceService
{
    /// <summary>
    /// Validates EN 1090 execution class
    /// </summary>
    /// <param name="executionClass">The execution class to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValidExecutionClass(string? executionClass)
    {
        if (string.IsNullOrWhiteSpace(executionClass))
            return false;

        return EN1090Constants.ExecutionClasses.All.Contains(executionClass.ToUpperInvariant());
    }

    /// <summary>
    /// Gets the project tier for an execution class
    /// </summary>
    /// <param name="executionClass">The execution class</param>
    /// <returns>The project tier (1, 2, or 3) or null if invalid</returns>
    public int? GetProjectTier(string? executionClass)
    {
        if (string.IsNullOrWhiteSpace(executionClass))
            return null;

        return EN1090Constants.ProjectTiers.GetTierFromExecutionClass(executionClass.ToUpperInvariant());
    }

    /// <summary>
    /// Validates EN 10204 certificate type
    /// </summary>
    /// <param name="certificateType">The certificate type to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValidCertificateType(string? certificateType)
    {
        if (string.IsNullOrWhiteSpace(certificateType))
            return true; // Optional field

        return EN1090Constants.CertificateTypes.All.Contains(certificateType);
    }

    /// <summary>
    /// Gets EN 1090 compliance requirements for a project tier
    /// </summary>
    /// <param name="tier">The project tier</param>
    /// <returns>Dictionary of compliance requirements</returns>
    public Dictionary<string, object> GetComplianceRequirements(int tier)
    {
        var requirements = new Dictionary<string, object>
        {
            ["Tier"] = tier,
            ["ExecutionClasses"] = EN1090Constants.ProjectTiers.GetExecutionClassesForTier(tier)
        };

        switch (tier)
        {
            case EN1090Constants.ProjectTiers.Tier1:
                requirements["MaterialTraceability"] = "Basic";
                requirements["WeldingProcedures"] = "Standard";
                requirements["TestCertificates"] = "EN 10204 2.1 minimum";
                requirements["Documentation"] = "Standard documentation required";
                break;

            case EN1090Constants.ProjectTiers.Tier2:
                requirements["MaterialTraceability"] = "Enhanced with batch tracking";
                requirements["WeldingProcedures"] = "Qualified procedures required";
                requirements["TestCertificates"] = "EN 10204 3.1 minimum";
                requirements["Documentation"] = "Enhanced documentation and inspection records";
                break;

            case EN1090Constants.ProjectTiers.Tier3:
                requirements["MaterialTraceability"] = "Full traceability with country of origin";
                requirements["WeldingProcedures"] = "Fully qualified procedures with PQR";
                requirements["TestCertificates"] = "EN 10204 3.2 required";
                requirements["Documentation"] = "Complete documentation package including NDT";
                requirements["QualityControl"] = "Independent inspection required";
                break;
        }

        return requirements;
    }

    /// <summary>
    /// Validates that material data meets EN 1090 requirements for a given tier
    /// </summary>
    /// <param name="tier">The project tier</param>
    /// <param name="materialBatch">Material batch number</param>
    /// <param name="certificateType">Certificate type</param>
    /// <param name="countryOfOrigin">Country of origin</param>
    /// <returns>Validation result with any issues</returns>
    public (bool IsValid, List<string> Issues) ValidateMaterialCompliance(
        int tier, 
        string? materialBatch, 
        string? certificateType, 
        string? countryOfOrigin)
    {
        var issues = new List<string>();

        // Validate certificate type if provided
        if (!string.IsNullOrWhiteSpace(certificateType) && !IsValidCertificateType(certificateType))
        {
            issues.Add($"Invalid certificate type: {certificateType}");
        }

        // Tier-specific validations
        switch (tier)
        {
            case EN1090Constants.ProjectTiers.Tier2:
                if (string.IsNullOrWhiteSpace(materialBatch))
                {
                    issues.Add("Material batch number is required for EXC3 projects");
                }
                if (string.IsNullOrWhiteSpace(certificateType))
                {
                    issues.Add("Certificate type is required for EXC3 projects (EN 10204 3.1 minimum)");
                }
                else if (certificateType == EN1090Constants.CertificateTypes.Type21)
                {
                    issues.Add("Certificate type 3.1 or higher is required for EXC3 projects");
                }
                break;

            case EN1090Constants.ProjectTiers.Tier3:
                if (string.IsNullOrWhiteSpace(materialBatch))
                {
                    issues.Add("Material batch number is required for EXC4 projects");
                }
                if (string.IsNullOrWhiteSpace(certificateType))
                {
                    issues.Add("Certificate type is required for EXC4 projects (EN 10204 3.2 required)");
                }
                else if (certificateType != EN1090Constants.CertificateTypes.Type32)
                {
                    issues.Add("Certificate type 3.2 is required for EXC4 projects");
                }
                if (string.IsNullOrWhiteSpace(countryOfOrigin))
                {
                    issues.Add("Country of origin is required for EXC4 projects");
                }
                break;
        }

        return (issues.Count == 0, issues);
    }
}