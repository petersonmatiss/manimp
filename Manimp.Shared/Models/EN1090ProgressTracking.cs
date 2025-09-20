using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Enumeration of assembly progress statuses according to EN 1090 workflow
/// </summary>
public enum AssemblyProgressStatus
{
    /// <summary>
    /// Initial status when assembly is created
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Assembly has been assembled
    /// </summary>
    Assembled = 1,

    /// <summary>
    /// Assembly has been welded
    /// </summary>
    Welded = 2,

    /// <summary>
    /// Assembly is ready for coating
    /// </summary>
    ReadyForCoating = 3,

    /// <summary>
    /// Coating has been completed (either in-house or outsourced)
    /// </summary>
    CoatingDone = 4,

    /// <summary>
    /// Assembly is ready for delivery
    /// </summary>
    ReadyForDelivery = 5,

    /// <summary>
    /// Assembly has been delivered
    /// </summary>
    Delivered = 6
}

/// <summary>
/// Enumeration of quality assurance test types
/// </summary>
public enum QualityAssuranceType
{
    /// <summary>
    /// Visual Testing as per EN 1090
    /// </summary>
    VisualTesting = 1,

    /// <summary>
    /// Dimensional inspection
    /// </summary>
    DimensionalInspection = 2,

    /// <summary>
    /// Weld quality inspection
    /// </summary>
    WeldQualityInspection = 3,

    /// <summary>
    /// Coating quality inspection
    /// </summary>
    CoatingQualityInspection = 4,

    /// <summary>
    /// Final inspection before delivery
    /// </summary>
    FinalInspection = 5
}

/// <summary>
/// Result status of quality assurance checks
/// </summary>
public enum QAResult
{
    /// <summary>
    /// Quality check passed
    /// </summary>
    Pass = 1,

    /// <summary>
    /// Quality check failed
    /// </summary>
    Fail = 2,

    /// <summary>
    /// Quality check passed with minor issues noted
    /// </summary>
    PassWithNotes = 3
}

/// <summary>
/// Represents the progress status history of an assembly through EN 1090 workflow
/// </summary>
public class AssemblyStatusHistory
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyStatusHistoryId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the previous status
    /// </summary>
    public AssemblyProgressStatus PreviousStatus { get; set; }

    /// <summary>
    /// Gets or sets the new status
    /// </summary>
    public AssemblyProgressStatus NewStatus { get; set; }

    /// <summary>
    /// Gets or sets when the status change occurred
    /// </summary>
    public DateTime ChangedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets who changed the status
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ChangedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets notes about the status change
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether all required QA checks were completed before this status change
    /// </summary>
    public bool QACompleted { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;
}

/// <summary>
/// Represents quality assurance records for assemblies according to EN 1090
/// </summary>
public class QualityAssuranceRecord
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int QualityAssuranceRecordId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the type of quality assurance check
    /// </summary>
    public QualityAssuranceType QAType { get; set; }

    /// <summary>
    /// Gets or sets the result of the quality check
    /// </summary>
    public QAResult Result { get; set; }

    /// <summary>
    /// Gets or sets when the QA check was performed
    /// </summary>
    public DateTime PerformedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets who performed the QA check
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string PerformedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assembly status this QA check is for
    /// </summary>
    public AssemblyProgressStatus ForStatus { get; set; }

    /// <summary>
    /// Gets or sets detailed findings and observations
    /// </summary>
    [MaxLength(2000)]
    public string? Findings { get; set; }

    /// <summary>
    /// Gets or sets corrective actions if any issues were found
    /// </summary>
    [MaxLength(1000)]
    public string? CorrectiveActions { get; set; }

    /// <summary>
    /// Gets or sets reference to EN 1090 standard section
    /// </summary>
    [MaxLength(50)]
    public string? EN1090Reference { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;
}

/// <summary>
/// Represents non-compliance reports (NCR) according to EN 1090 requirements
/// </summary>
public class NonComplianceReport
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int NonComplianceReportId { get; set; }

    /// <summary>
    /// Gets or sets the NCR number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string NCRNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets when the non-compliance was detected
    /// </summary>
    public DateTime DetectedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets who detected the non-compliance
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DetectedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category of non-compliance
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity level (Critical, Major, Minor)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets detailed description of the non-compliance
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the root cause analysis
    /// </summary>
    [MaxLength(1000)]
    public string? RootCause { get; set; }

    /// <summary>
    /// Gets or sets immediate corrective actions taken
    /// </summary>
    [MaxLength(1000)]
    public string? ImmediateActions { get; set; }

    /// <summary>
    /// Gets or sets preventive actions to avoid recurrence
    /// </summary>
    [MaxLength(1000)]
    public string? PreventiveActions { get; set; }

    /// <summary>
    /// Gets or sets the current status of the NCR
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Open";

    /// <summary>
    /// Gets or sets who is responsible for resolving the NCR
    /// </summary>
    [MaxLength(100)]
    public string? ResponsiblePerson { get; set; }

    /// <summary>
    /// Gets or sets the target resolution date
    /// </summary>
    public DateTime? TargetResolutionDate { get; set; }

    /// <summary>
    /// Gets or sets when the NCR was resolved
    /// </summary>
    public DateTime? ResolvedUtc { get; set; }

    /// <summary>
    /// Gets or sets who resolved the NCR
    /// </summary>
    [MaxLength(100)]
    public string? ResolvedBy { get; set; }

    /// <summary>
    /// Gets or sets resolution notes
    /// </summary>
    [MaxLength(1000)]
    public string? ResolutionNotes { get; set; }

    /// <summary>
    /// Gets or sets EN 1090 standard reference
    /// </summary>
    [MaxLength(100)]
    public string? EN1090Reference { get; set; }

    /// <summary>
    /// Gets or sets whether customer notification is required
    /// </summary>
    public bool CustomerNotificationRequired { get; set; }

    /// <summary>
    /// Gets or sets when customer was notified
    /// </summary>
    public DateTime? CustomerNotifiedUtc { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;
}

/// <summary>
/// Represents outsourced coating tracking for assemblies
/// </summary>
public class OutsourcedCoatingTracking
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int OutsourcedCoatingTrackingId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the supplier identifier for the coating company
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets when the assembly was sent for coating
    /// </summary>
    public DateTime SentDate { get; set; }

    /// <summary>
    /// Gets or sets the expected return date
    /// </summary>
    public DateTime ExpectedReturnDate { get; set; }

    /// <summary>
    /// Gets or sets when the assembly was actually returned
    /// </summary>
    public DateTime? ActualReturnDate { get; set; }

    /// <summary>
    /// Gets or sets the current status of outsourced coating
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Sent";

    /// <summary>
    /// Gets or sets the coating specification
    /// </summary>
    [MaxLength(500)]
    public string? CoatingSpecification { get; set; }

    /// <summary>
    /// Gets or sets the cost of outsourced coating
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Gets or sets tracking notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets who sent the assembly for coating
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string SentBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets who received the assembly back
    /// </summary>
    [MaxLength(100)]
    public string? ReceivedBy { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;

    /// <summary>
    /// Gets the coating supplier
    /// </summary>
    public Supplier Supplier { get; set; } = null!;
}