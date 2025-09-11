using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Represents a tenant in the multi-tenant system
/// </summary>
public class Tenant
{
    /// <summary>
    /// Gets or sets the unique identifier for the tenant
    /// </summary>
    public Guid TenantId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the tenant name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database name for this tenant
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret reference for database connection
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string SecretRef { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the tenant is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the UTC creation date
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Maps user emails to their tenant
/// </summary>
public class UserDirectory
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the normalized email address
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string NormalizedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant identifier
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the associated tenant
    /// </summary>
    public Tenant? Tenant { get; set; }
}

/// <summary>
/// Request model for company registration
/// </summary>
public class CompanyRegistrationRequest
{
    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the admin email address
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string AdminEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the admin password
    /// </summary>
    [Required]
    [MinLength(6)]
    public string AdminPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating a new user
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    [MaxLength(100)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    [MaxLength(100)]
    public string? LastName { get; set; }
}

// Tier 1 Core Inventory Schema

// Lookup Tables

/// <summary>
/// Represents a material type classification (e.g., structural steel, plate steel)
/// </summary>
public class MaterialType
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int MaterialTypeId { get; set; }

    /// <summary>
    /// Gets or sets the material type name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this material type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the profile inventories using this material type
    /// </summary>
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new HashSet<ProfileInventory>();

    /// <summary>
    /// Gets the purchase order lines using this material type
    /// </summary>
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new HashSet<PurchaseOrderLine>();
}

/// <summary>
/// Represents a profile shape type (e.g., W-beam, channel, angle)
/// </summary>
public class ProfileType
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int ProfileTypeId { get; set; }

    /// <summary>
    /// Gets or sets the profile type name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this profile type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the profile inventories using this profile type
    /// </summary>
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new HashSet<ProfileInventory>();

    /// <summary>
    /// Gets the purchase order lines using this profile type
    /// </summary>
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new HashSet<PurchaseOrderLine>();
}

/// <summary>
/// Represents a steel grade specification (e.g., A992, A36, A572-50)
/// </summary>
public class SteelGrade
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int SteelGradeId { get; set; }

    /// <summary>
    /// Gets or sets the steel grade name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this steel grade is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the profile inventories using this steel grade
    /// </summary>
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new HashSet<ProfileInventory>();

    /// <summary>
    /// Gets the purchase order lines using this steel grade
    /// </summary>
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new HashSet<PurchaseOrderLine>();
}

// Supporting entities for tagging

/// <summary>
/// Represents a supplier/vendor for inventory tracking
/// </summary>
public class Supplier
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the supplier name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional contact information
    /// </summary>
    [MaxLength(1000)]
    public string? ContactInfo { get; set; }

    /// <summary>
    /// Gets or sets whether this supplier is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the profile inventories from this supplier
    /// </summary>
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new HashSet<ProfileInventory>();

    /// <summary>
    /// Gets the purchase orders from this supplier
    /// </summary>
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new HashSet<PurchaseOrder>();
}

/// <summary>
/// Represents a project for usage tracking and reporting
/// </summary>
public class Project
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional project description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this project is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the optional start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the optional end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the profile usage logs associated with this project
    /// </summary>
    public ICollection<ProfileUsageLog> ProfileUsageLogs { get; set; } = new HashSet<ProfileUsageLog>();

    /// <summary>
    /// Gets the profile inventories directly associated with this project
    /// </summary>
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new HashSet<ProfileInventory>();
}

/// <summary>
/// Represents a document (certificate, invoice, etc.) associated with inventory
/// </summary>
public class Document
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the file name
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the document type
    /// </summary>
    [MaxLength(50)]
    public string? DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the file path for storage
    /// </summary>
    [MaxLength(1000)]
    public string? FilePath { get; set; }

    /// <summary>
    /// Gets or sets the UTC upload date
    /// </summary>
    public DateTime UploadedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the profile inventories referencing this document
    /// </summary>
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new HashSet<ProfileInventory>();
}

// Core Inventory and Usage Tracking

/// <summary>
/// Represents individual inventory items/lots with piece tracking
/// </summary>
public class ProfileInventory
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int ProfileInventoryId { get; set; }

    /// <summary>
    /// Gets or sets the lot number
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LotNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size specification (e.g., "W12x26", "L4x4x1/2")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Size { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the length in feet or meters
    /// </summary>
    [Range(0.001, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
    public decimal Length { get; set; }

    /// <summary>
    /// Gets or sets the current available pieces
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Pieces on hand cannot be negative")]
    public int PiecesOnHand { get; set; }

    /// <summary>
    /// Gets or sets the original quantity received
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Original pieces must be at least 1")]
    public int OriginalPieces { get; set; }

    /// <summary>
    /// Gets or sets the weight per piece in lbs or kg
    /// </summary>
    [Range(0.001, double.MaxValue, ErrorMessage = "Weight per piece must be greater than 0")]
    public decimal WeightPerPiece { get; set; }

    /// <summary>
    /// Gets or sets the optional unit cost
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost cannot be negative")]
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// Gets or sets the date received
    /// </summary>
    public DateTime ReceivedDate { get; set; }

    /// <summary>
    /// Gets or sets the optional warehouse location
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Foreign Keys
    /// <summary>
    /// Gets or sets the material type identifier
    /// </summary>
    public int MaterialTypeId { get; set; }

    /// <summary>
    /// Gets or sets the profile type identifier
    /// </summary>
    public int ProfileTypeId { get; set; }

    /// <summary>
    /// Gets or sets the steel grade identifier
    /// </summary>
    public int SteelGradeId { get; set; }

    /// <summary>
    /// Gets or sets the optional supplier identifier
    /// </summary>
    public int? SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the optional certificate document identifier
    /// </summary>
    public int? CertificateDocumentId { get; set; }

    // Tier 2 Procurement and Project tracking fields
    /// <summary>
    /// Gets or sets the optional purchase order identifier for procurement lineage
    /// </summary>
    public int? PurchaseOrderId { get; set; }

    /// <summary>
    /// Gets or sets the optional purchase order number for reference
    /// </summary>
    [MaxLength(50)]
    public string? PONumber { get; set; }

    /// <summary>
    /// Gets or sets the optional project identifier for direct project association
    /// </summary>
    public int? ProjectId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the material type
    /// </summary>
    public MaterialType MaterialType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the profile type
    /// </summary>
    public ProfileType ProfileType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the steel grade
    /// </summary>
    public SteelGrade SteelGrade { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional supplier
    /// </summary>
    public Supplier? Supplier { get; set; }

    /// <summary>
    /// Gets or sets the optional certificate document
    /// </summary>
    public Document? CertificateDocument { get; set; }

    /// <summary>
    /// Gets or sets the optional purchase order for procurement lineage
    /// </summary>
    public PurchaseOrder? PurchaseOrder { get; set; }

    /// <summary>
    /// Gets or sets the optional project for direct project association
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// Gets the usage logs for this inventory item
    /// </summary>
    public ICollection<ProfileUsageLog> ProfileUsageLogs { get; set; } = new HashSet<ProfileUsageLog>();

    /// <summary>
    /// Gets the remnant inventories created from this inventory item
    /// </summary>
    public ICollection<ProfileRemnantInventory> ProfileRemnantInventories { get; set; } = new HashSet<ProfileRemnantInventory>();
}

/// <summary>
/// Represents usage tracking with automatic inventory decrements
/// </summary>
public class ProfileUsageLog
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int ProfileUsageLogId { get; set; }

    /// <summary>
    /// Gets or sets the number of pieces used
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Pieces used must be at least 1")]
    public int PiecesUsed { get; set; }

    /// <summary>
    /// Gets or sets the optional length used for partial tracking
    /// </summary>
    [Range(0.001, double.MaxValue, ErrorMessage = "Length used must be greater than 0")]
    public decimal? LengthUsed { get; set; }

    /// <summary>
    /// Gets or sets the date used
    /// </summary>
    public DateTime UsedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the optional purpose description
    /// </summary>
    [MaxLength(200)]
    public string? Purpose { get; set; }

    /// <summary>
    /// Gets or sets who used the material
    /// </summary>
    [MaxLength(100)]
    public string? UsedBy { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Foreign Keys
    /// <summary>
    /// Gets or sets the profile inventory identifier
    /// </summary>
    public int ProfileInventoryId { get; set; }

    /// <summary>
    /// Gets or sets the optional project identifier
    /// </summary>
    public int? ProjectId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the profile inventory item
    /// </summary>
    public ProfileInventory ProfileInventory { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional project
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// Gets the remnant inventories created from this usage
    /// </summary>
    public ICollection<ProfileRemnantInventory> ProfileRemnantInventories { get; set; } = new HashSet<ProfileRemnantInventory>();
}

// Tier 2 Procurement and Remnants Module

/// <summary>
/// Represents a purchase order for procurement tracking
/// </summary>
public class PurchaseOrder
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PurchaseOrderId { get; set; }

    /// <summary>
    /// Gets or sets the purchase order number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string PONumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the purchase order date
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the expected delivery date
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the actual delivery date
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the total order amount
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative")]
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the purchase order status
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Foreign Keys
    /// <summary>
    /// Gets or sets the optional supplier identifier
    /// </summary>
    public int? SupplierId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the optional supplier
    /// </summary>
    public Supplier? Supplier { get; set; }

    /// <summary>
    /// Gets the purchase order lines
    /// </summary>
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new HashSet<PurchaseOrderLine>();

    /// <summary>
    /// Gets the profile inventories received from this purchase order
    /// </summary>
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new HashSet<ProfileInventory>();
}

/// <summary>
/// Represents individual line items on a purchase order
/// </summary>
public class PurchaseOrderLine
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PurchaseOrderLineId { get; set; }

    /// <summary>
    /// Gets or sets the line number on the purchase order
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Line number must be at least 1")]
    public int LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the size specification (e.g., "W12x26", "L4x4x1/2")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Size { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the length in feet or meters
    /// </summary>
    [Range(0.001, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
    public decimal Length { get; set; }

    /// <summary>
    /// Gets or sets the quantity ordered
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the line total amount
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Line total cannot be negative")]
    public decimal? LineTotal { get; set; }

    /// <summary>
    /// Gets or sets optional description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Foreign Keys
    /// <summary>
    /// Gets or sets the purchase order identifier
    /// </summary>
    public int PurchaseOrderId { get; set; }

    /// <summary>
    /// Gets or sets the material type identifier
    /// </summary>
    public int MaterialTypeId { get; set; }

    /// <summary>
    /// Gets or sets the profile type identifier
    /// </summary>
    public int ProfileTypeId { get; set; }

    /// <summary>
    /// Gets or sets the steel grade identifier
    /// </summary>
    public int SteelGradeId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the purchase order
    /// </summary>
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the material type
    /// </summary>
    public MaterialType MaterialType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the profile type
    /// </summary>
    public ProfileType ProfileType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the steel grade
    /// </summary>
    public SteelGrade SteelGrade { get; set; } = null!;
}

/// <summary>
/// Represents remnant inventory created when material usage leaves leftover length
/// </summary>
public class ProfileRemnantInventory
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int ProfileRemnantInventoryId { get; set; }

    /// <summary>
    /// Gets or sets the remnant lot number (auto-generated)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string RemnantLotNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the remaining length
    /// </summary>
    [Range(0.001, double.MaxValue, ErrorMessage = "Remaining length must be greater than 0")]
    public decimal RemainingLength { get; set; }

    /// <summary>
    /// Gets or sets the number of remnant pieces
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Remnant pieces must be at least 1")]
    public int RemnantPieces { get; set; }

    /// <summary>
    /// Gets or sets the date the remnant was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the optional warehouse location
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether this remnant is available for use
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Foreign Keys
    /// <summary>
    /// Gets or sets the original profile inventory identifier
    /// </summary>
    public int OriginalProfileInventoryId { get; set; }

    /// <summary>
    /// Gets or sets the profile usage log that created this remnant
    /// </summary>
    public int ProfileUsageLogId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the original profile inventory item
    /// </summary>
    public ProfileInventory OriginalProfileInventory { get; set; } = null!;

    /// <summary>
    /// Gets or sets the profile usage log that created this remnant
    /// </summary>
    public ProfileUsageLog ProfileUsageLog { get; set; } = null!;
}
