using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

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
    [Required]
    [MaxLength(20)]
    [RegularExpression("^(Pending|Ordered|Delivered|Cancelled|Completed)$",
        ErrorMessage = "Status must be one of: Pending, Ordered, Delivered, Cancelled, Completed")]
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

    /// <summary>
    /// Gets or sets the optional price request line identifier for sourcing lineage
    /// </summary>
    public int? PriceRequestLineId { get; set; }

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

    /// <summary>
    /// Gets or sets the optional price request line for sourcing lineage
    /// </summary>
    public PriceRequestLine? PriceRequestLine { get; set; }
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