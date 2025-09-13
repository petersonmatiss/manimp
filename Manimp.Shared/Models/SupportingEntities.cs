using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

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

    /// <summary>
    /// Gets the price requests for this supplier
    /// </summary>
    public ICollection<PriceRequest> PriceRequests { get; set; } = new HashSet<PriceRequest>();

    /// <summary>
    /// Gets the price quotes from this supplier
    /// </summary>
    public ICollection<PriceQuote> PriceQuotes { get; set; } = new HashSet<PriceQuote>();
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
    /// Gets or sets the EN 1090 execution class (EXC1, EXC2, EXC3, EXC4)
    /// </summary>
    [MaxLength(10)]
    public string? ExecutionClass { get; set; }

    /// <summary>
    /// Gets or sets the project tier based on execution class (1=EXC1-2, 2=EXC3, 3=EXC4)
    /// </summary>
    public int? ProjectTier { get; set; }

    /// <summary>
    /// Gets or sets the month when this project was created (YYYY-MM format)
    /// </summary>
    [MaxLength(7)]
    public string? CreatedMonth { get; set; }

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

    /// <summary>
    /// Gets the price requests associated with this project
    /// </summary>
    public ICollection<PriceRequest> PriceRequests { get; set; } = new HashSet<PriceRequest>();
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