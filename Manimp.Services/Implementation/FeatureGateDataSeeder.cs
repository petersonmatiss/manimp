using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Manimp.Directory.Data;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

/// <summary>
/// Service for seeding initial feature gating data
/// </summary>
public class FeatureGateDataSeeder
{
    private readonly DirectoryDbContext _directoryDb;
    private readonly ILogger<FeatureGateDataSeeder> _logger;

    public FeatureGateDataSeeder(DirectoryDbContext directoryDb, ILogger<FeatureGateDataSeeder> logger)
    {
        _directoryDb = directoryDb;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the initial plans, features, and plan-feature mappings
    /// </summary>
    public async Task SeedInitialDataAsync()
    {
        try
        {
            await SeedFeaturesAsync();
            await _directoryDb.SaveChangesAsync();

            await SeedPlansAsync();
            await _directoryDb.SaveChangesAsync();

            await SeedPlanFeaturesAsync();
            await _directoryDb.SaveChangesAsync();

            _logger.LogInformation("Feature gating data seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding feature gating data");
            throw;
        }
    }

    /// <summary>
    /// Seeds the initial features
    /// </summary>
    private async Task SeedFeaturesAsync()
    {
        var features = new[]
        {
            // Tier 1 Core Inventory Features
            new Feature
            {
                FeatureKey = FeatureKeys.CoreInventory,
                Name = "Core Inventory Management",
                Description = "Basic inventory tracking and management",
                Category = "Inventory"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.ProfileManagement,
                Name = "Profile Management",
                Description = "Manage material profiles and specifications",
                Category = "Inventory"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.UsageTracking,
                Name = "Usage Tracking",
                Description = "Track material usage and consumption",
                Category = "Inventory"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.MaterialLookups,
                Name = "Material Lookups",
                Description = "Access to material type, profile, and grade lookups",
                Category = "Inventory"
            },

            // Tier 2 Procurement and Remnants Features
            new Feature
            {
                FeatureKey = FeatureKeys.ProcurementManagement,
                Name = "Procurement Management",
                Description = "Advanced procurement tracking and management",
                Category = "Procurement"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.PurchaseOrders,
                Name = "Purchase Orders",
                Description = "Create and manage purchase orders",
                Category = "Procurement"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.RemnantTracking,
                Name = "Remnant Tracking",
                Description = "Automated tracking of material remnants",
                Category = "Procurement"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.ProcurementReports,
                Name = "Procurement Reports",
                Description = "Advanced reporting for procurement activities",
                Category = "Procurement"
            },

            // Tier 3 Sourcing Features
            new Feature
            {
                FeatureKey = FeatureKeys.SourcingManagement,
                Name = "Sourcing Management",
                Description = "Strategic sourcing and vendor management",
                Category = "Sourcing"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.PriceRequests,
                Name = "Price Requests",
                Description = "Request for quote (RFQ) management",
                Category = "Sourcing"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.QuoteManagement,
                Name = "Quote Management",
                Description = "Manage vendor quotes and comparisons",
                Category = "Sourcing"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.VendorComparison,
                Name = "Vendor Comparison",
                Description = "Compare vendors and pricing across quotes",
                Category = "Sourcing"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.SourcingReports,
                Name = "Sourcing Reports",
                Description = "Advanced reporting for sourcing activities",
                Category = "Sourcing"
            },

            // Future module placeholders
            new Feature
            {
                FeatureKey = FeatureKeys.ProjectManagement,
                Name = "Project Management",
                Description = "Enhanced project management and tracking",
                Category = "Projects"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.QualityControl,
                Name = "Quality Control",
                Description = "Quality control and inspection management",
                Category = "Quality"
            },
            new Feature
            {
                FeatureKey = FeatureKeys.ProductionTracking,
                Name = "Production Tracking",
                Description = "Manufacturing and production tracking",
                Category = "Production"
            }
        };

        foreach (var feature in features)
        {
            var existing = await _directoryDb.Features
                .FirstOrDefaultAsync(f => f.FeatureKey == feature.FeatureKey);

            if (existing == null)
            {
                _directoryDb.Features.Add(feature);
                _logger.LogDebug("Added feature: {FeatureKey}", feature.FeatureKey);
            }
        }
    }

    /// <summary>
    /// Seeds the initial subscription plans
    /// </summary>
    private async Task SeedPlansAsync()
    {
        var plans = new[]
        {
            new Plan
            {
                Name = "Basic",
                Description = "Basic inventory management for small operations",
                TierLevel = 1
            },
            new Plan
            {
                Name = "Professional",
                Description = "Advanced procurement and remnant tracking for growing businesses",
                TierLevel = 2
            },
            new Plan
            {
                Name = "Enterprise",
                Description = "Complete sourcing and vendor management for large operations",
                TierLevel = 3
            }
        };

        foreach (var plan in plans)
        {
            var existing = await _directoryDb.Plans
                .FirstOrDefaultAsync(p => p.Name == plan.Name);

            if (existing == null)
            {
                _directoryDb.Plans.Add(plan);
                _logger.LogDebug("Added plan: {PlanName}", plan.Name);
            }
        }
    }

    /// <summary>
    /// Seeds the plan-feature mappings
    /// </summary>
    private async Task SeedPlanFeaturesAsync()
    {
        // Get the plans and features from database
        var basicPlan = await _directoryDb.Plans.FirstAsync(p => p.Name == "Basic");
        var professionalPlan = await _directoryDb.Plans.FirstAsync(p => p.Name == "Professional");
        var enterprisePlan = await _directoryDb.Plans.FirstAsync(p => p.Name == "Enterprise");

        var features = await _directoryDb.Features.ToListAsync();
        var featureMap = features.ToDictionary(f => f.FeatureKey, f => f);

        // Define plan-feature mappings
        var planFeatureMappings = new Dictionary<Plan, string[]>
        {
            [basicPlan] = new[]
            {
                FeatureKeys.CoreInventory,
                FeatureKeys.ProfileManagement,
                FeatureKeys.UsageTracking,
                FeatureKeys.MaterialLookups
            },
            [professionalPlan] = new[]
            {
                // All Basic features
                FeatureKeys.CoreInventory,
                FeatureKeys.ProfileManagement,
                FeatureKeys.UsageTracking,
                FeatureKeys.MaterialLookups,
                // Plus Professional features
                FeatureKeys.ProcurementManagement,
                FeatureKeys.PurchaseOrders,
                FeatureKeys.RemnantTracking,
                FeatureKeys.ProcurementReports
            },
            [enterprisePlan] = new[]
            {
                // All Basic and Professional features
                FeatureKeys.CoreInventory,
                FeatureKeys.ProfileManagement,
                FeatureKeys.UsageTracking,
                FeatureKeys.MaterialLookups,
                FeatureKeys.ProcurementManagement,
                FeatureKeys.PurchaseOrders,
                FeatureKeys.RemnantTracking,
                FeatureKeys.ProcurementReports,
                // Plus Enterprise features
                FeatureKeys.SourcingManagement,
                FeatureKeys.PriceRequests,
                FeatureKeys.QuoteManagement,
                FeatureKeys.VendorComparison,
                FeatureKeys.SourcingReports,
                // Future features available in Enterprise
                FeatureKeys.ProjectManagement,
                FeatureKeys.QualityControl,
                FeatureKeys.ProductionTracking
            }
        };

        // Create plan-feature relationships
        foreach (var (plan, featureKeys) in planFeatureMappings)
        {
            foreach (var featureKey in featureKeys)
            {
                if (featureMap.TryGetValue(featureKey, out var feature))
                {
                    var existing = await _directoryDb.PlanFeatures
                        .FirstOrDefaultAsync(pf => pf.PlanId == plan.PlanId && pf.FeatureId == feature.FeatureId);

                    if (existing == null)
                    {
                        _directoryDb.PlanFeatures.Add(new PlanFeature
                        {
                            PlanId = plan.PlanId,
                            FeatureId = feature.FeatureId,
                            IsEnabled = true
                        });

                        _logger.LogDebug("Added feature {FeatureKey} to plan {PlanName}", featureKey, plan.Name);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Assigns the default Basic plan to a tenant
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    public async Task AssignDefaultPlanToTenantAsync(Guid tenantId)
    {
        var basicPlan = await _directoryDb.Plans.FirstOrDefaultAsync(p => p.Name == "Basic");
        if (basicPlan == null)
        {
            _logger.LogError("Basic plan not found, cannot assign to tenant {TenantId}", tenantId);
            return;
        }

        var existingSubscription = await _directoryDb.TenantSubscriptions
            .FirstOrDefaultAsync(ts => ts.TenantId == tenantId && ts.IsActive);

        if (existingSubscription == null)
        {
            _directoryDb.TenantSubscriptions.Add(new TenantSubscription
            {
                TenantId = tenantId,
                PlanId = basicPlan.PlanId,
                StartDate = DateTime.UtcNow,
                IsActive = true
            });

            await _directoryDb.SaveChangesAsync();
            _logger.LogInformation("Assigned Basic plan to tenant {TenantId}", tenantId);
        }
    }
}