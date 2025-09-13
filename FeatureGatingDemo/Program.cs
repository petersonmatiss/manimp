using Microsoft.EntityFrameworkCore;
using Manimp.Directory.Data;
using Manimp.Services.Implementation;
using Manimp.Shared.Models;

namespace FeatureGatingDemo;

/// <summary>
/// Console application to demonstrate feature gating functionality
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Manimp Feature Gating System Demo ===\n");

        try
        {
            // Setup in-memory database for demo
            var options = new DbContextOptionsBuilder<DirectoryDbContext>()
                .UseInMemoryDatabase("FeatureGatingDemo")
                .Options;

            using var dbContext = new DirectoryDbContext(options);
            
            // Seed initial data
            var seeder = new FeatureGateDataSeeder(dbContext, 
                Microsoft.Extensions.Logging.Abstractions.NullLogger<FeatureGateDataSeeder>.Instance);
            
            try
            {
                await seeder.SeedInitialDataAsync();
                Console.WriteLine("✅ Seeded feature gating data successfully");
                
                // Verify seeding worked
                var planCount = await dbContext.Plans.CountAsync();
                var featureCount = await dbContext.Features.CountAsync();
                Console.WriteLine($"   • Plans: {planCount}");
                Console.WriteLine($"   • Features: {featureCount}\n");
            }
            catch (Exception seedEx)
            {
                Console.WriteLine($"❌ Seeding error: {seedEx.Message}");
                throw;
            }

            // Create demo tenants with different subscription plans
            var tenants = await CreateDemoTenantsAsync(dbContext, seeder);
            
            // Create feature gate service
            var featureGate = new FeatureGateService(dbContext, 
                Microsoft.Extensions.Logging.Abstractions.NullLogger<FeatureGateService>.Instance);

            // Demonstrate feature checking for each tenant
            await DemonstrateFeatureGating(featureGate, tenants);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner: {ex.InnerException.Message}");
            }
        }
    }

    private static async Task<List<Guid>> CreateDemoTenantsAsync(DirectoryDbContext dbContext, FeatureGateDataSeeder seeder)
    {
        Console.WriteLine("📊 Creating demo tenants with different subscription plans...\n");

        var tenants = new List<Guid>();

        // Create tenants for each plan type
        var plans = await dbContext.Plans.ToListAsync();
        
        if (!plans.Any())
        {
            throw new InvalidOperationException("No plans found. Ensure seeding completed successfully.");
        }
        
        foreach (var plan in plans)
        {
            var tenantId = Guid.NewGuid();
            
            var tenant = new Tenant
            {
                TenantId = tenantId,
                Name = $"Demo Company ({plan.Name} Plan)",
                DbName = $"demo_{plan.Name.ToLower()}_{tenantId:N}",
                SecretRef = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            };

            dbContext.Tenants.Add(tenant);

            var subscription = new TenantSubscription
            {
                TenantId = tenantId,
                PlanId = plan.PlanId,
                StartDate = DateTime.UtcNow,
                IsActive = true
            };

            dbContext.TenantSubscriptions.Add(subscription);
            tenants.Add(tenantId);

            Console.WriteLine($"   • Created {plan.Name} tenant: {tenant.Name}");
        }

        // Create a tenant with an override for demonstration
        var overrideTenantId = Guid.NewGuid();
        var basicPlan = await dbContext.Plans.FirstAsync(p => p.Name == "Basic");
        var sourcingFeature = await dbContext.Features.FirstAsync(f => f.FeatureKey == FeatureKeys.SourcingManagement);

        var overrideTenant = new Tenant
        {
            TenantId = overrideTenantId,
            Name = "Demo Company (Basic + Sourcing Override)",
            DbName = $"demo_override_{overrideTenantId:N}",
            SecretRef = Guid.NewGuid().ToString(),
            IsActive = true,
            CreatedUtc = DateTime.UtcNow
        };

        dbContext.Tenants.Add(overrideTenant);

        var overrideSubscription = new TenantSubscription
        {
            TenantId = overrideTenantId,
            PlanId = basicPlan.PlanId,
            StartDate = DateTime.UtcNow,
            IsActive = true
        };

        dbContext.TenantSubscriptions.Add(overrideSubscription);

        var featureOverride = new TenantFeatureOverride
        {
            TenantId = overrideTenantId,
            FeatureId = sourcingFeature.FeatureId,
            IsEnabled = true,
            Reason = "Trial access to Sourcing features",
            CreatedUtc = DateTime.UtcNow
        };

        dbContext.TenantFeatureOverrides.Add(featureOverride);
        tenants.Add(overrideTenantId);

        Console.WriteLine($"   • Created override tenant: {overrideTenant.Name}");

        await dbContext.SaveChangesAsync();
        Console.WriteLine($"\n✅ Created {tenants.Count} demo tenants\n");

        return tenants;
    }

    private static async Task DemonstrateFeatureGating(FeatureGateService featureGate, List<Guid> tenantIds)
    {
        Console.WriteLine("🔒 Demonstrating Feature Gating...\n");

        var testFeatures = new[]
        {
            FeatureKeys.CoreInventory,
            FeatureKeys.PurchaseOrders,
            FeatureKeys.RemnantTracking,
            FeatureKeys.SourcingManagement,
            FeatureKeys.PriceRequests
        };

        foreach (var tenantId in tenantIds)
        {
            Console.WriteLine($"📋 Tenant: {tenantId}");
            
            var allFeatures = await featureGate.GetTenantFeatureStatusesAsync(tenantId);
            
            foreach (var feature in testFeatures)
            {
                if (allFeatures.TryGetValue(feature, out var status))
                {
                    var enabledIcon = status.IsEnabled ? "✅" : "❌";
                    var sourceInfo = status.Source == "Override" ? " (Override)" : 
                                   status.Source == "Plan" ? " (Plan)" : " (Default)";
                    
                    Console.WriteLine($"   {enabledIcon} {GetFeatureName(feature)}{sourceInfo}");
                }
            }
            Console.WriteLine();
        }

        // Demonstrate middleware-like functionality
        Console.WriteLine("🚪 Testing Middleware-like Feature Checks...\n");
        
        var firstTenant = tenantIds.First();
        await TestFeatureAccessScenarios(featureGate, firstTenant);
    }

    private static async Task TestFeatureAccessScenarios(FeatureGateService featureGate, Guid tenantId)
    {
        var scenarios = new[]
        {
            (FeatureKeys.CoreInventory, "Basic Inventory Access"),
            (FeatureKeys.PurchaseOrders, "Purchase Order Management"),
            (FeatureKeys.SourcingManagement, "Advanced Sourcing Features"),
            ("nonexistent_feature", "Non-existent Feature")
        };

        foreach (var (featureKey, description) in scenarios)
        {
            var isEnabled = await featureGate.IsFeatureEnabledAsync(tenantId, featureKey);
            var result = isEnabled ? "ALLOWED" : "DENIED";
            var icon = isEnabled ? "✅" : "❌";
            
            Console.WriteLine($"   {icon} {description}: {result}");
        }
    }

    private static string GetFeatureName(string featureKey)
    {
        return featureKey switch
        {
            FeatureKeys.CoreInventory => "Core Inventory",
            FeatureKeys.PurchaseOrders => "Purchase Orders",
            FeatureKeys.RemnantTracking => "Remnant Tracking",
            FeatureKeys.SourcingManagement => "Sourcing Management",
            FeatureKeys.PriceRequests => "Price Requests",
            _ => featureKey.Replace("_", " ")
        };
    }
}