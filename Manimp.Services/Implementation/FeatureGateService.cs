using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Manimp.Directory.Data;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

/// <summary>
/// Service for checking tenant feature access based on subscriptions and overrides
/// </summary>
public class FeatureGateService : IFeatureGate
{
    private readonly DirectoryDbContext _directoryDb;
    private readonly ILogger<FeatureGateService> _logger;

    public FeatureGateService(DirectoryDbContext directoryDb, ILogger<FeatureGateService> logger)
    {
        _directoryDb = directoryDb;
        _logger = logger;
    }

    /// <summary>
    /// Checks if a specific feature is enabled for a tenant
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="featureKey">The feature key to check</param>
    /// <returns>True if the feature is enabled, false otherwise</returns>
    public async Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureKey)
    {
        try
        {
            var status = await GetFeatureStatusAsync(tenantId, featureKey);
            return status.IsEnabled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking feature {FeatureKey} for tenant {TenantId}", featureKey, tenantId);
            return false; // Fail closed - deny access on error
        }
    }

    /// <summary>
    /// Gets the detailed status of a feature for a tenant
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="featureKey">The feature key to check</param>
    /// <returns>The feature status with metadata</returns>
    public async Task<FeatureStatus> GetFeatureStatusAsync(Guid tenantId, string featureKey)
    {
        // Check for tenant-specific override first
        var override_ = await _directoryDb.TenantFeatureOverrides
            .Include(tfo => tfo.Feature)
            .FirstOrDefaultAsync(tfo =>
                tfo.TenantId == tenantId &&
                tfo.Feature.FeatureKey == featureKey &&
                tfo.Feature.IsActive &&
                (tfo.ExpiresUtc == null || tfo.ExpiresUtc > DateTime.UtcNow));

        if (override_ != null)
        {
            return new FeatureStatus
            {
                FeatureKey = featureKey,
                IsEnabled = override_.IsEnabled,
                Source = "Override",
                Metadata = new Dictionary<string, object>
                {
                    ["overrideId"] = override_.TenantFeatureOverrideId,
                    ["reason"] = override_.Reason ?? string.Empty,
                    ["expiresUtc"] = override_.ExpiresUtc?.ToString("O") ?? string.Empty
                }
            };
        }

        // Check tenant subscription and plan features
        var planFeature = await _directoryDb.TenantSubscriptions
            .Where(ts =>
                ts.TenantId == tenantId &&
                ts.IsActive &&
                (ts.EndDate == null || ts.EndDate > DateTime.UtcNow))
            .Join(_directoryDb.PlanFeatures,
                ts => ts.PlanId,
                pf => pf.PlanId,
                (ts, pf) => new { ts, pf })
            .Join(_directoryDb.Features,
                x => x.pf.FeatureId,
                f => f.FeatureId,
                (x, f) => new { x.ts, x.pf, f })
            .Where(x => x.f.FeatureKey == featureKey && x.f.IsActive)
            .Select(x => new { x.ts, x.pf, x.f })
            .FirstOrDefaultAsync();

        if (planFeature != null)
        {
            return new FeatureStatus
            {
                FeatureKey = featureKey,
                IsEnabled = planFeature.pf.IsEnabled,
                Source = "Plan",
                Metadata = new Dictionary<string, object>
                {
                    ["planId"] = planFeature.ts.PlanId,
                    ["subscriptionId"] = planFeature.ts.TenantSubscriptionId,
                    ["planFeatureId"] = planFeature.pf.PlanFeatureId
                }
            };
        }

        // Feature not found in subscription or override - default to disabled
        return new FeatureStatus
        {
            FeatureKey = featureKey,
            IsEnabled = false,
            Source = "Default",
            Metadata = new Dictionary<string, object>
            {
                ["reason"] = "Feature not found in tenant subscription or override"
            }
        };
    }

    /// <summary>
    /// Gets all features and their enabled status for a tenant
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Dictionary of feature keys to enabled status</returns>
    public async Task<Dictionary<string, bool>> GetTenantFeaturesAsync(Guid tenantId)
    {
        var statuses = await GetTenantFeatureStatusesAsync(tenantId);
        return statuses.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IsEnabled);
    }

    /// <summary>
    /// Gets all features and their detailed status for a tenant
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Dictionary of feature keys to feature status</returns>
    public async Task<Dictionary<string, FeatureStatus>> GetTenantFeatureStatusesAsync(Guid tenantId)
    {
        var result = new Dictionary<string, FeatureStatus>();

        // Get all active features
        var allFeatures = await _directoryDb.Features
            .Where(f => f.IsActive)
            .Select(f => f.FeatureKey)
            .ToListAsync();

        // Get tenant overrides
        var overrides = await _directoryDb.TenantFeatureOverrides
            .Include(tfo => tfo.Feature)
            .Where(tfo =>
                tfo.TenantId == tenantId &&
                tfo.Feature.IsActive &&
                (tfo.ExpiresUtc == null || tfo.ExpiresUtc > DateTime.UtcNow))
            .ToDictionaryAsync(tfo => tfo.Feature.FeatureKey, tfo => tfo);

        // Get plan features from active subscriptions
        var planFeatures = await _directoryDb.TenantSubscriptions
            .Where(ts =>
                ts.TenantId == tenantId &&
                ts.IsActive &&
                (ts.EndDate == null || ts.EndDate > DateTime.UtcNow))
            .Join(_directoryDb.PlanFeatures,
                ts => ts.PlanId,
                pf => pf.PlanId,
                (ts, pf) => new { ts, pf })
            .Join(_directoryDb.Features,
                x => x.pf.FeatureId,
                f => f.FeatureId,
                (x, f) => new { x.ts, x.pf, f })
            .Where(x => x.f.IsActive)
            .ToDictionaryAsync(
                x => x.f.FeatureKey,
                x => new { x.ts, x.pf, x.f });

        // Process each feature
        foreach (var featureKey in allFeatures)
        {
            FeatureStatus status;

            // Check override first
            if (overrides.TryGetValue(featureKey, out var override_))
            {
                status = new FeatureStatus
                {
                    FeatureKey = featureKey,
                    IsEnabled = override_.IsEnabled,
                    Source = "Override",
                    Metadata = new Dictionary<string, object>
                    {
                        ["overrideId"] = override_.TenantFeatureOverrideId,
                        ["reason"] = override_.Reason ?? string.Empty,
                        ["expiresUtc"] = override_.ExpiresUtc?.ToString("O") ?? string.Empty
                    }
                };
            }
            // Check plan feature
            else if (planFeatures.TryGetValue(featureKey, out var planFeature))
            {
                status = new FeatureStatus
                {
                    FeatureKey = featureKey,
                    IsEnabled = planFeature.pf.IsEnabled,
                    Source = "Plan",
                    Metadata = new Dictionary<string, object>
                    {
                        ["planId"] = planFeature.ts.PlanId,
                        ["subscriptionId"] = planFeature.ts.TenantSubscriptionId,
                        ["planFeatureId"] = planFeature.pf.PlanFeatureId
                    }
                };
            }
            // Default to disabled
            else
            {
                status = new FeatureStatus
                {
                    FeatureKey = featureKey,
                    IsEnabled = false,
                    Source = "Default",
                    Metadata = new Dictionary<string, object>
                    {
                        ["reason"] = "Feature not found in tenant subscription or override"
                    }
                };
            }

            result[featureKey] = status;
        }

        return result;
    }
}