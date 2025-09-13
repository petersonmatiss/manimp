using Manimp.Shared.Models;

namespace Manimp.Services;

/// <summary>
/// Interface for managing tenant project limits
/// </summary>
public interface IProjectLimitService
{
    /// <summary>
    /// Checks if a tenant can create a new project this month
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>True if the tenant can create a project, false otherwise</returns>
    Task<bool> CanCreateProjectAsync(Guid tenantId);

    /// <summary>
    /// Gets the remaining project slots for the current month
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The number of remaining project slots</returns>
    Task<int> GetRemainingProjectsAsync(Guid tenantId);

    /// <summary>
    /// Increments the project count for a tenant (call when a project is created)
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>True if the project was counted successfully, false if limit exceeded</returns>
    Task<bool> IncrementProjectCountAsync(Guid tenantId);

    /// <summary>
    /// Decrements the project count for a tenant (call when a project is deleted)
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>True if the project was decremented successfully</returns>
    Task<bool> DecrementProjectCountAsync(Guid tenantId);

    /// <summary>
    /// Adds addon projects to a tenant's monthly limit
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="additionalProjects">Number of additional projects to add</param>
    /// <returns>The updated project limit record</returns>
    Task<TenantProjectLimit> AddAddonProjectsAsync(Guid tenantId, int additionalProjects);

    /// <summary>
    /// Gets the project limits for a specific tenant and month
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="month">The month in yyyy-MM format</param>
    /// <returns>The project limit record</returns>
    Task<TenantProjectLimit> GetProjectLimitAsync(Guid tenantId, string month);

    /// <summary>
    /// Gets project limit history for a tenant
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="monthsBack">Number of months back to retrieve (default: 12)</param>
    /// <returns>List of project limits</returns>
    Task<List<TenantProjectLimit>> GetProjectLimitHistoryAsync(Guid tenantId, int monthsBack = 12);
}

/// <summary>
/// Interface for EN 1090 compliance validation and subscription tier management
/// </summary>
public interface IEN1090ComplianceService
{
    /// <summary>
    /// Validates EN 1090 execution class
    /// </summary>
    /// <param name="executionClass">The execution class to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidExecutionClass(string? executionClass);

    /// <summary>
    /// Gets the minimum subscription tier required for an execution class
    /// </summary>
    /// <param name="executionClass">The execution class</param>
    /// <returns>The minimum subscription tier required</returns>
    int GetRequiredSubscriptionTier(string? executionClass);

    /// <summary>
    /// Checks if a tenant's subscription tier allows a specific execution class
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="executionClass">The execution class to check</param>
    /// <returns>True if allowed, false otherwise</returns>
    Task<bool> IsExecutionClassAllowedAsync(Guid tenantId, string? executionClass);

    /// <summary>
    /// Gets the tenant's current subscription tier for EN 1090 features
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The subscription tier (1=Basic, 2=Professional, 3=Enterprise)</returns>
    Task<int> GetTenantSubscriptionTierAsync(Guid tenantId);

    /// <summary>
    /// Validates EN 10204 certificate type
    /// </summary>
    /// <param name="certificateType">The certificate type to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidCertificateType(string? certificateType);

    /// <summary>
    /// Gets EN 1090 compliance requirements for a subscription tier
    /// </summary>
    /// <param name="subscriptionTier">The subscription tier</param>
    /// <returns>Dictionary of compliance requirements</returns>
    Dictionary<string, object> GetComplianceRequirements(int subscriptionTier);

    /// <summary>
    /// Validates that material data meets EN 1090 requirements for a given subscription tier
    /// </summary>
    /// <param name="subscriptionTier">The subscription tier</param>
    /// <param name="materialBatch">Material batch number</param>
    /// <param name="certificateType">Certificate type</param>
    /// <param name="countryOfOrigin">Country of origin</param>
    /// <returns>Validation result with any issues</returns>
    (bool IsValid, List<string> Issues) ValidateMaterialCompliance(
        int subscriptionTier, 
        string? materialBatch, 
        string? certificateType, 
        string? countryOfOrigin);
}