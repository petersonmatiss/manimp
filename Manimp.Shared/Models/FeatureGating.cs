using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Represents a subscription plan with available features
/// </summary>
public class Plan
{
    /// <summary>
    /// Gets or sets the unique identifier for the plan
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// Gets or sets the plan name (e.g., "Basic", "Professional", "Enterprise")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plan description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the plan tier level (1 = Basic, 2 = Professional, 3 = Enterprise)
    /// </summary>
    public int TierLevel { get; set; }

    /// <summary>
    /// Gets or sets whether the plan is active and available for subscription
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the plan was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the plan features
    /// </summary>
    public ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();

    /// <summary>
    /// Gets or sets the tenant subscriptions
    /// </summary>
    public ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();
}

/// <summary>
/// Represents a feature that can be enabled/disabled for tenants
/// </summary>
public class Feature
{
    /// <summary>
    /// Gets or sets the unique identifier for the feature
    /// </summary>
    public int FeatureId { get; set; }

    /// <summary>
    /// Gets or sets the feature key (used in code to check feature access)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FeatureKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable feature name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the feature description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the feature category (e.g., "Inventory", "Procurement", "Sourcing")
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets whether the feature is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the feature was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the plan features
    /// </summary>
    public ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();

    /// <summary>
    /// Gets or sets the tenant feature overrides
    /// </summary>
    public ICollection<TenantFeatureOverride> TenantFeatureOverrides { get; set; } = new List<TenantFeatureOverride>();
}

/// <summary>
/// Links plans to their available features
/// </summary>
public class PlanFeature
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PlanFeatureId { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// Gets or sets the feature identifier
    /// </summary>
    public int FeatureId { get; set; }

    /// <summary>
    /// Gets or sets whether the feature is enabled in this plan
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets when this plan feature was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the associated plan
    /// </summary>
    public Plan Plan { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated feature
    /// </summary>
    public Feature Feature { get; set; } = null!;
}

/// <summary>
/// Represents a tenant's subscription to a plan
/// </summary>
public class TenantSubscription
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int TenantSubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// Gets or sets when the subscription started
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when the subscription ends (null for perpetual)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether the subscription is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when this subscription was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the associated tenant
    /// </summary>
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated plan
    /// </summary>
    public Plan Plan { get; set; } = null!;
}

/// <summary>
/// Allows per-tenant overrides of feature access (for trials, custom features, etc.)
/// </summary>
public class TenantFeatureOverride
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int TenantFeatureOverrideId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the feature identifier
    /// </summary>
    public int FeatureId { get; set; }

    /// <summary>
    /// Gets or sets whether the feature is enabled for this tenant (overrides plan settings)
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the reason for the override
    /// </summary>
    [MaxLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets when the override expires (null for permanent)
    /// </summary>
    public DateTime? ExpiresUtc { get; set; }

    /// <summary>
    /// Gets or sets when this override was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the associated tenant
    /// </summary>
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated feature
    /// </summary>
    public Feature Feature { get; set; } = null!;
}

/// <summary>
/// Constants for feature keys used throughout the application
/// </summary>
public static class FeatureKeys
{
    // Tier 1 Core Inventory Features
    public const string CoreInventory = "core_inventory";
    public const string ProfileManagement = "profile_management";
    public const string UsageTracking = "usage_tracking";
    public const string MaterialLookups = "material_lookups";

    // Tier 2 Procurement and Remnants Features
    public const string ProcurementManagement = "procurement_management";
    public const string PurchaseOrders = "purchase_orders";
    public const string RemnantTracking = "remnant_tracking";
    public const string ProcurementReports = "procurement_reports";

    // Tier 3 Sourcing Features
    public const string SourcingManagement = "sourcing_management";
    public const string PriceRequests = "price_requests";
    public const string QuoteManagement = "quote_management";
    public const string VendorComparison = "vendor_comparison";
    public const string SourcingReports = "sourcing_reports";

    // CRM Module Features
    public const string CrmModule = "crm_module";
    public const string CustomerManagement = "customer_management";
    public const string ContactManagement = "contact_management";
    public const string CrmProjectManagement = "crm_project_management";
    public const string AssemblyManagement = "assembly_management";
    public const string PartManagement = "part_management";
    public const string BoltManagement = "bolt_management";
    public const string CoatingManagement = "coating_management";
    public const string OutsourcingManagement = "outsourcing_management";
    public const string CuttingListOptimization = "cutting_list_optimization";
    public const string DeliveryManagement = "delivery_management";
    public const string PackingListGeneration = "packing_list_generation";

    // Tiered File Upload Features
    public const string AssemblyListUpload = "assembly_list_upload";
    public const string AssemblyListUploadTier2 = "assembly_list_upload_tier2"; // Manual column mapping
    public const string AssemblyListUploadTier3 = "assembly_list_upload_tier3"; // AI-powered parsing

    // Manufacturing Progress Tracking
    public const string ManufacturingProgress = "manufacturing_progress";
    public const string ProgressReporting = "progress_reporting";

    // Future module placeholders
    public const string QualityControl = "quality_control";
    public const string ProductionTracking = "production_tracking";
}

/// <summary>
/// DTO for feature status and metadata
/// </summary>
public class FeatureStatus
{
    /// <summary>
    /// Gets or sets the feature key
    /// </summary>
    public string FeatureKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the feature is enabled for the tenant
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the source of the feature status (Plan, Override, Default)
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional metadata about the feature
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}