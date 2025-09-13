using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Represents a price request for sourcing materials from suppliers
/// </summary>
public class PriceRequest
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PriceRequestId { get; set; }

    /// <summary>
    /// Gets or sets the price request number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string RequestNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price request date
    /// </summary>
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the required by date
    /// </summary>
    public DateTime? RequiredByDate { get; set; }

    /// <summary>
    /// Gets or sets the price request status
    /// </summary>
    [Required]
    [MaxLength(20)]
    [RegularExpression("^(Draft|Sent|Quoted|Completed|Cancelled)$",
        ErrorMessage = "Status must be one of: Draft, Sent, Quoted, Completed, Cancelled")]
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Gets or sets optional notes for the request
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Foreign Keys
    /// <summary>
    /// Gets or sets the optional supplier identifier for directed requests
    /// </summary>
    public int? SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the optional project identifier
    /// </summary>
    public int? ProjectId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the optional supplier for directed requests
    /// </summary>
    public Supplier? Supplier { get; set; }

    /// <summary>
    /// Gets or sets the optional project
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// Gets the price request lines
    /// </summary>
    public ICollection<PriceRequestLine> PriceRequestLines { get; set; } = new HashSet<PriceRequestLine>();

    /// <summary>
    /// Gets the price quotes received for this request
    /// </summary>
    public ICollection<PriceQuote> PriceQuotes { get; set; } = new HashSet<PriceQuote>();
}

/// <summary>
/// Represents individual line items on a price request
/// </summary>
public class PriceRequestLine
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PriceRequestLineId { get; set; }

    /// <summary>
    /// Gets or sets the line number on the price request
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
    /// Gets or sets the quantity requested
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

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
    /// Gets or sets the price request identifier
    /// </summary>
    public int PriceRequestId { get; set; }

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
    /// Gets or sets the price request
    /// </summary>
    public PriceRequest PriceRequest { get; set; } = null!;

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
    /// Gets the purchase order lines created from this price request line
    /// </summary>
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new HashSet<PurchaseOrderLine>();
}

/// <summary>
/// Represents price quotes received from suppliers in response to price requests
/// </summary>
public class PriceQuote
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PriceQuoteId { get; set; }

    /// <summary>
    /// Gets or sets the quote number from the supplier
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string QuoteNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the quote was received
    /// </summary>
    public DateTime QuoteDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the quote expiration date
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the total quote amount
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative")]
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the quote status
    /// </summary>
    [Required]
    [MaxLength(20)]
    [RegularExpression("^(Received|Under Review|Accepted|Rejected|Expired)$",
        ErrorMessage = "Status must be one of: Received, Under Review, Accepted, Rejected, Expired")]
    public string Status { get; set; } = "Received";

    /// <summary>
    /// Gets or sets optional notes about the quote
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Foreign Keys
    /// <summary>
    /// Gets or sets the price request identifier
    /// </summary>
    public int PriceRequestId { get; set; }

    /// <summary>
    /// Gets or sets the supplier identifier
    /// </summary>
    public int SupplierId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the price request
    /// </summary>
    public PriceRequest PriceRequest { get; set; } = null!;

    /// <summary>
    /// Gets or sets the supplier
    /// </summary>
    public Supplier Supplier { get; set; } = null!;
}