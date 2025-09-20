using System.ComponentModel.DataAnnotations;
using Manimp.Shared.Models;

namespace Manimp.Shared.DTOs;

/// <summary>
/// Request model for updating assembly status
/// </summary>
public class UpdateStatusRequest
{
    [Required]
    public AssemblyProgressStatus NewStatus { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ChangedBy { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for adding QA record
/// </summary>
public class AddQARecordRequest
{
    [Required]
    public QualityAssuranceType QAType { get; set; }

    [Required]
    public QAResult Result { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string PerformedBy { get; set; } = string.Empty;

    [Required]
    public AssemblyProgressStatus ForStatus { get; set; }

    [StringLength(1000)]
    public string? Findings { get; set; }

    [StringLength(1000)]
    public string? CorrectiveActions { get; set; }

    [StringLength(100)]
    public string? EN1090Reference { get; set; }
}

/// <summary>
/// Request model for creating NCR
/// </summary>
public class CreateNCRRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string NCRNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string DetectedBy { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Severity { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string? EN1090Reference { get; set; }
}

/// <summary>
/// Request model for creating outsourced coating tracking
/// </summary>
public class CreateOutsourcedCoatingRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Supplier ID must be a positive number")]
    public int SupplierId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string SentBy { get; set; } = string.Empty;

    [Required]
    public DateTime ExpectedReturnDate { get; set; }

    [StringLength(500)]
    public string? CoatingSpecification { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost must be a positive number")]
    public decimal? Cost { get; set; }
}