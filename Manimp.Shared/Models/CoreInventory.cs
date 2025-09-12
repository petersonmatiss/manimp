using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

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