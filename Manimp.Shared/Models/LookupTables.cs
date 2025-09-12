using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

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