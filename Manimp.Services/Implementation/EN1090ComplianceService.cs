using Microsoft.Extensions.Logging;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

/// <summary>
/// Service for EN 1090 compliance validation based on tenant subscription tiers
/// </summary>
public class EN1090ComplianceService : IEN1090ComplianceService
{
    private readonly IFeatureGate _featureGate;
    private readonly ILogger<EN1090ComplianceService> _logger;

    public EN1090ComplianceService(IFeatureGate featureGate, ILogger<EN1090ComplianceService> logger)
    {
        _featureGate = featureGate;
        _logger = logger;
    }

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
    /// Gets the minimum subscription tier required for an execution class
    /// </summary>
    /// <param name="executionClass">The execution class</param>
    /// <returns>The minimum subscription tier required</returns>
    public int GetRequiredSubscriptionTier(string? executionClass)
    {
        return EN1090Constants.SubscriptionTiers.GetRequiredSubscriptionTier(executionClass);
    }

    /// <summary>
    /// Checks if a tenant's subscription tier allows a specific execution class
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="executionClass">The execution class to check</param>
    /// <returns>True if allowed, false otherwise</returns>
    public async Task<bool> IsExecutionClassAllowedAsync(Guid tenantId, string? executionClass)
    {
        try
        {
            // First check if EN 1090 compliance feature is enabled
            var hasEN1090Feature = await _featureGate.IsFeatureEnabledAsync(tenantId, FeatureKeys.EN1090Compliance);
            if (!hasEN1090Feature)
            {
                _logger.LogWarning("Tenant {TenantId} does not have EN 1090 compliance feature enabled", tenantId);
                return false;
            }

            if (!IsValidExecutionClass(executionClass))
            {
                return false;
            }

            var tenantTier = await GetTenantSubscriptionTierAsync(tenantId);
            var requiredTier = GetRequiredSubscriptionTier(executionClass);

            return tenantTier >= requiredTier;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking execution class {ExecutionClass} for tenant {TenantId}", executionClass, tenantId);
            return false;
        }
    }

    /// <summary>
    /// Gets the tenant's current subscription tier for EN 1090 features
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The subscription tier (1=Basic, 2=Professional, 3=Enterprise)</returns>
    public async Task<int> GetTenantSubscriptionTierAsync(Guid tenantId)
    {
        try
        {
            // Get all feature statuses for the tenant
            var featureStatuses = await _featureGate.GetTenantFeatureStatusesAsync(tenantId);

            // Determine tier based on available features
            // Check for highest tier features first
            if (featureStatuses.ContainsKey(FeatureKeys.EN1090Compliance) &&
                featureStatuses[FeatureKeys.EN1090Compliance].IsEnabled)
            {
                // Check tier-specific features to determine subscription level
                if (featureStatuses.GetValueOrDefault(FeatureKeys.CrmModule)?.IsEnabled == true ||
                    featureStatuses.GetValueOrDefault(FeatureKeys.SourcingManagement)?.IsEnabled == true)
                {
                    return EN1090Constants.SubscriptionTiers.Enterprise;
                }
                else if (featureStatuses.GetValueOrDefault(FeatureKeys.ProcurementManagement)?.IsEnabled == true ||
                         featureStatuses.GetValueOrDefault(FeatureKeys.RemnantTracking)?.IsEnabled == true)
                {
                    return EN1090Constants.SubscriptionTiers.Professional;
                }
                else
                {
                    return EN1090Constants.SubscriptionTiers.Basic;
                }
            }

            return EN1090Constants.SubscriptionTiers.Basic; // Default to basic if EN1090 not enabled
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining subscription tier for tenant {TenantId}", tenantId);
            return EN1090Constants.SubscriptionTiers.Basic; // Fail to most restrictive tier
        }
    }

    /// <summary>
    /// Validates EN 10204 certificate type
    /// </summary>
    /// <param name="certificateType">The certificate type to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValidCertificateType(string? certificateType)
    {
        if (string.IsNullOrWhiteSpace(certificateType))
            return false;

        return EN1090Constants.CertificateTypes.All.Contains(certificateType);
    }

    /// <summary>
    /// Gets EN 1090 compliance requirements for a subscription tier
    /// </summary>
    /// <param name="subscriptionTier">The subscription tier</param>
    /// <returns>Dictionary of compliance requirements</returns>
    public Dictionary<string, object> GetComplianceRequirements(int subscriptionTier)
    {
        var requirements = new Dictionary<string, object>
        {
            ["SubscriptionTier"] = subscriptionTier,
            ["TierName"] = EN1090Constants.SubscriptionTiers.GetTierName(subscriptionTier),
            ["AllowedExecutionClasses"] = EN1090Constants.SubscriptionTiers.GetAllowedExecutionClasses(subscriptionTier)
        };

        switch (subscriptionTier)
        {
            case EN1090Constants.SubscriptionTiers.Basic:
                requirements["MaterialTraceability"] = "Basic documentation";
                requirements["WeldingProcedures"] = "Standard procedures";
                requirements["TestCertificates"] = "EN 10204 2.1 minimum";
                requirements["Documentation"] = "Standard project documentation";
                requirements["ExecutionClasses"] = "EXC1, EXC2";
                requirements["Features"] = new[] { "Basic inventory", "Simple project tracking" };
                break;

            case EN1090Constants.SubscriptionTiers.Professional:
                requirements["MaterialTraceability"] = "Enhanced with batch tracking";
                requirements["WeldingProcedures"] = "Qualified procedures required";
                requirements["TestCertificates"] = "EN 10204 3.1 minimum";
                requirements["Documentation"] = "Enhanced documentation and inspection records";
                requirements["ExecutionClasses"] = "EXC1, EXC2, EXC3";
                requirements["Features"] = new[] { "Full inventory management", "Procurement features", "Advanced project tracking" };
                break;

            case EN1090Constants.SubscriptionTiers.Enterprise:
                requirements["MaterialTraceability"] = "Full traceability with country of origin";
                requirements["WeldingProcedures"] = "Fully qualified procedures with PQR";
                requirements["TestCertificates"] = "EN 10204 3.2 required";
                requirements["Documentation"] = "Complete documentation package including NDT";
                requirements["QualityControl"] = "Independent inspection required";
                requirements["ExecutionClasses"] = "EXC1, EXC2, EXC3, EXC4 (all classes)";
                requirements["Features"] = new[] { "Full CRM", "Sourcing management", "Complete compliance suite", "Advanced analytics" };
                break;
        }

        return requirements;
    }

    /// <summary>
    /// Validates that material data meets EN 1090 requirements for a given subscription tier
    /// </summary>
    /// <param name="subscriptionTier">The subscription tier</param>
    /// <param name="materialBatch">Material batch number</param>
    /// <param name="certificateType">Certificate type</param>
    /// <param name="countryOfOrigin">Country of origin</param>
    /// <returns>Validation result with any issues</returns>
    public (bool IsValid, List<string> Issues) ValidateMaterialCompliance(
        int subscriptionTier, 
        string? materialBatch, 
        string? certificateType, 
        string? countryOfOrigin)
    {
        var issues = new List<string>();

        switch (subscriptionTier)
        {
            case EN1090Constants.SubscriptionTiers.Basic:
                // Basic tier - minimal requirements
                if (!string.IsNullOrWhiteSpace(certificateType) && !IsValidCertificateType(certificateType))
                {
                    issues.Add("Invalid certificate type. Must be EN 10204 2.1, 2.2, 3.1, or 3.2");
                }
                break;

            case EN1090Constants.SubscriptionTiers.Professional:
                // Professional tier - enhanced requirements
                if (string.IsNullOrWhiteSpace(materialBatch))
                {
                    issues.Add("Material batch number is required for Professional tier projects");
                }

                if (string.IsNullOrWhiteSpace(certificateType))
                {
                    issues.Add("Test certificate type is required for Professional tier projects");
                }
                else if (!IsValidCertificateType(certificateType))
                {
                    issues.Add("Invalid certificate type. Must be EN 10204 2.1, 2.2, 3.1, or 3.2");
                }
                else if (certificateType == EN1090Constants.CertificateTypes.Type21)
                {
                    issues.Add("Professional tier requires EN 10204 3.1 minimum certificate");
                }
                break;

            case EN1090Constants.SubscriptionTiers.Enterprise:
                // Enterprise tier - full requirements
                if (string.IsNullOrWhiteSpace(materialBatch))
                {
                    issues.Add("Material batch number is mandatory for Enterprise tier projects");
                }

                if (string.IsNullOrWhiteSpace(certificateType))
                {
                    issues.Add("Test certificate type is mandatory for Enterprise tier projects");
                }
                else if (!IsValidCertificateType(certificateType))
                {
                    issues.Add("Invalid certificate type. Must be EN 10204 2.1, 2.2, 3.1, or 3.2");
                }
                else if (certificateType != EN1090Constants.CertificateTypes.Type32)
                {
                    issues.Add("Enterprise tier requires EN 10204 3.2 certificate for full traceability");
                }

                if (string.IsNullOrWhiteSpace(countryOfOrigin))
                {
                    issues.Add("Country of origin is mandatory for Enterprise tier projects");
                }
                break;

            default:
                issues.Add($"Unknown subscription tier: {subscriptionTier}");
                break;
        }

        return (issues.Count == 0, issues);
    }
}