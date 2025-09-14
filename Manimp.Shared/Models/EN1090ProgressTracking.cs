using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Manufacturing steps according to EN 1090 standards
/// </summary>
public enum ManufacturingStep
{
    /// <summary>
    /// Initial state - not yet started
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Assembly step - parts are assembled together
    /// </summary>
    Assembled = 1,

    /// <summary>
    /// Welding step - welding operations completed
    /// </summary>
    Welded = 2,

    /// <summary>
    /// Ready for coating - assembly prepared for coating
    /// </summary>
    ReadyForCoating = 3,

    /// <summary>
    /// Coating done - coating operations completed
    /// </summary>
    CoatingDone = 4,

    /// <summary>
    /// Ready for delivery - final preparations complete
    /// </summary>
    ReadyForDelivery = 5,

    /// <summary>
    /// Delivered - assembly delivered to customer
    /// </summary>
    Delivered = 6
}

/// <summary>
/// Quality check types as per EN 1090 requirements
/// </summary>
public enum QualityCheckType
{
    /// <summary>
    /// Visual Testing (VT) - visual inspection
    /// </summary>
    VisualTesting = 1,

    /// <summary>
    /// Quality Assurance (QA) - quality control check
    /// </summary>
    QualityAssurance = 2,

    /// <summary>
    /// Dimensional check - measurements and dimensions
    /// </summary>
    DimensionalCheck = 3,

    /// <summary>
    /// Weld quality check - specific to welding operations
    /// </summary>
    WeldQualityCheck = 4,

    /// <summary>
    /// Coating quality check - specific to coating operations
    /// </summary>
    CoatingQualityCheck = 5,

    /// <summary>
    /// Final inspection - before delivery
    /// </summary>
    FinalInspection = 6
}

/// <summary>
/// Status of quality checks
/// </summary>
public enum QualityCheckStatus
{
    /// <summary>
    /// Not yet performed
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Currently in progress
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Passed the quality check
    /// </summary>
    Passed = 2,

    /// <summary>
    /// Failed the quality check
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Failed but with accepted deviation
    /// </summary>
    FailedAccepted = 4
}

/// <summary>
/// Severity levels for non-compliance records
/// </summary>
public enum NonComplianceSeverity
{
    /// <summary>
    /// Minor non-compliance that doesn't affect function
    /// </summary>
    Minor = 1,

    /// <summary>
    /// Major non-compliance that may affect function
    /// </summary>
    Major = 2,

    /// <summary>
    /// Critical non-compliance that definitely affects function or safety
    /// </summary>
    Critical = 3
}

/// <summary>
/// Status of non-compliance records
/// </summary>
public enum NonComplianceStatus
{
    /// <summary>
    /// NCR opened and under investigation
    /// </summary>
    Open = 1,

    /// <summary>
    /// Under review by quality team
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Corrective action in progress
    /// </summary>
    CorrectiveActionInProgress = 3,

    /// <summary>
    /// Rework completed, awaiting verification
    /// </summary>
    AwaitingVerification = 4,

    /// <summary>
    /// NCR closed - resolved
    /// </summary>
    Closed = 5,

    /// <summary>
    /// NCR closed with concession - accepted as-is
    /// </summary>
    ClosedWithConcession = 6
}

/// <summary>
/// Tracks progress through EN 1090 manufacturing steps for assemblies
/// </summary>
public class AssemblyProgress
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyProgressId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the current manufacturing step
    /// </summary>
    public ManufacturingStep CurrentStep { get; set; } = ManufacturingStep.NotStarted;

    /// <summary>
    /// Gets or sets the previous manufacturing step
    /// </summary>
    public ManufacturingStep? PreviousStep { get; set; }

    /// <summary>
    /// Gets or sets when the current step was started
    /// </summary>
    public DateTime? CurrentStepStarted { get; set; }

    /// <summary>
    /// Gets or sets when the current step was completed
    /// </summary>
    public DateTime? CurrentStepCompleted { get; set; }

    /// <summary>
    /// Gets or sets who updated the progress last
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets whether coating is outsourced for this assembly
    /// </summary>
    public bool IsCoatingOutsourced { get; set; } = false;

    /// <summary>
    /// Gets or sets when the assembly was sent for outsourced coating
    /// </summary>
    public DateTime? OutsourcedCoatingSentDate { get; set; }

    /// <summary>
    /// Gets or sets the expected return date from outsourced coating
    /// </summary>
    public DateTime? OutsourcedCoatingExpectedReturnDate { get; set; }

    /// <summary>
    /// Gets or sets the actual return date from outsourced coating
    /// </summary>
    public DateTime? OutsourcedCoatingActualReturnDate { get; set; }

    /// <summary>
    /// Gets or sets notes related to the current step
    /// </summary>
    [MaxLength(1000)]
    public string? StepNotes { get; set; }

    /// <summary>
    /// Gets or sets when this progress record was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this progress record was last updated
    /// </summary>
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

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
    /// Gets the quality checks for this assembly progress
    /// </summary>
    public ICollection<QualityCheck> QualityChecks { get; set; } = new HashSet<QualityCheck>();

    /// <summary>
    /// Gets the step history for this assembly
    /// </summary>
    public ICollection<AssemblyProgressStepHistory> StepHistory { get; set; } = new HashSet<AssemblyProgressStepHistory>();
}

/// <summary>
/// Tracks quality checks required by EN 1090 at each manufacturing step
/// </summary>
public class QualityCheck
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int QualityCheckId { get; set; }

    /// <summary>
    /// Gets or sets the assembly progress identifier
    /// </summary>
    public int AssemblyProgressId { get; set; }

    /// <summary>
    /// Gets or sets the type of quality check
    /// </summary>
    public QualityCheckType CheckType { get; set; }

    /// <summary>
    /// Gets or sets the manufacturing step this check is for
    /// </summary>
    public ManufacturingStep ForStep { get; set; }

    /// <summary>
    /// Gets or sets the status of the quality check
    /// </summary>
    public QualityCheckStatus Status { get; set; } = QualityCheckStatus.Pending;

    /// <summary>
    /// Gets or sets when the check was performed
    /// </summary>
    public DateTime? CheckedDate { get; set; }

    /// <summary>
    /// Gets or sets who performed the check
    /// </summary>
    [MaxLength(100)]
    public string? CheckedBy { get; set; }

    /// <summary>
    /// Gets or sets the check results
    /// </summary>
    [MaxLength(2000)]
    public string? CheckResults { get; set; }

    /// <summary>
    /// Gets or sets whether this check is required before proceeding
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Gets or sets reference to relevant standards or procedures
    /// </summary>
    [MaxLength(500)]
    public string? StandardReference { get; set; }

    /// <summary>
    /// Gets or sets any defects found during the check
    /// </summary>
    [MaxLength(1000)]
    public string? DefectsFound { get; set; }

    /// <summary>
    /// Gets or sets corrective actions taken
    /// </summary>
    [MaxLength(1000)]
    public string? CorrectiveActions { get; set; }

    /// <summary>
    /// Gets or sets when this quality check was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly progress
    /// </summary>
    public AssemblyProgress AssemblyProgress { get; set; } = null!;

    /// <summary>
    /// Gets the non-compliance records related to this quality check
    /// </summary>
    public ICollection<NonComplianceRecord> NonComplianceRecords { get; set; } = new HashSet<NonComplianceRecord>();
}

/// <summary>
/// Non-Compliance Record (NCR) as required by EN 1090
/// </summary>
public class NonComplianceRecord
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int NonComplianceRecordId { get; set; }

    /// <summary>
    /// Gets or sets the NCR number (unique reference)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string NCRNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quality check identifier (if related to a specific check)
    /// </summary>
    public int? QualityCheckId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the manufacturing step where non-compliance was found
    /// </summary>
    public ManufacturingStep DiscoveredAtStep { get; set; }

    /// <summary>
    /// Gets or sets the severity of the non-compliance
    /// </summary>
    public NonComplianceSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets the current status of the NCR
    /// </summary>
    public NonComplianceStatus Status { get; set; } = NonComplianceStatus.Open;

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
    /// Gets or sets the immediate corrective action taken
    /// </summary>
    [MaxLength(1000)]
    public string? ImmediateAction { get; set; }

    /// <summary>
    /// Gets or sets the preventive action to avoid recurrence
    /// </summary>
    [MaxLength(1000)]
    public string? PreventiveAction { get; set; }

    /// <summary>
    /// Gets or sets who discovered the non-compliance
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DiscoveredBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the non-compliance was discovered
    /// </summary>
    public DateTime DiscoveredDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets who is responsible for resolving the NCR
    /// </summary>
    [MaxLength(100)]
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets the target resolution date
    /// </summary>
    public DateTime? TargetResolutionDate { get; set; }

    /// <summary>
    /// Gets or sets the actual resolution date
    /// </summary>
    public DateTime? ActualResolutionDate { get; set; }

    /// <summary>
    /// Gets or sets who verified the resolution
    /// </summary>
    [MaxLength(100)]
    public string? VerifiedBy { get; set; }

    /// <summary>
    /// Gets or sets when the resolution was verified
    /// </summary>
    public DateTime? VerificationDate { get; set; }

    /// <summary>
    /// Gets or sets verification comments
    /// </summary>
    [MaxLength(1000)]
    public string? VerificationComments { get; set; }

    /// <summary>
    /// Gets or sets whether customer notification is required
    /// </summary>
    public bool CustomerNotificationRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets when customer was notified
    /// </summary>
    public DateTime? CustomerNotifiedDate { get; set; }

    /// <summary>
    /// Gets or sets customer response/approval
    /// </summary>
    [MaxLength(1000)]
    public string? CustomerResponse { get; set; }

    /// <summary>
    /// Gets or sets reference to relevant EN 1090 clause
    /// </summary>
    [MaxLength(100)]
    public string? EN1090Reference { get; set; }

    /// <summary>
    /// Gets or sets any additional documentation references
    /// </summary>
    [MaxLength(500)]
    public string? DocumentationReferences { get; set; }

    /// <summary>
    /// Gets or sets estimated cost impact
    /// </summary>
    public decimal? CostImpact { get; set; }

    /// <summary>
    /// Gets or sets estimated time impact (hours)
    /// </summary>
    public decimal? TimeImpactHours { get; set; }

    /// <summary>
    /// Gets or sets when this NCR was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this NCR was last updated
    /// </summary>
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated quality check (if applicable)
    /// </summary>
    public QualityCheck? QualityCheck { get; set; }

    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;
}

/// <summary>
/// Tracks the history of manufacturing step changes for an assembly
/// </summary>
public class AssemblyProgressStepHistory
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyProgressStepHistoryId { get; set; }

    /// <summary>
    /// Gets or sets the assembly progress identifier
    /// </summary>
    public int AssemblyProgressId { get; set; }

    /// <summary>
    /// Gets or sets the manufacturing step
    /// </summary>
    public ManufacturingStep Step { get; set; }

    /// <summary>
    /// Gets or sets when this step was started
    /// </summary>
    public DateTime StepStarted { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this step was completed
    /// </summary>
    public DateTime? StepCompleted { get; set; }

    /// <summary>
    /// Gets or sets who updated to this step
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets notes for this step
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets duration in hours for this step
    /// </summary>
    public decimal? DurationHours { get; set; }

    /// <summary>
    /// Gets or sets when this history record was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly progress
    /// </summary>
    public AssemblyProgress AssemblyProgress { get; set; } = null!;
}

/// <summary>
/// Tracks assemblies that are sent for outsourced coating
/// </summary>
public class OutsourcedCoatingList
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int OutsourcedCoatingListId { get; set; }

    /// <summary>
    /// Gets or sets the list name/reference
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ListName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the supplier identifier for coating service
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets when the assemblies were sent
    /// </summary>
    public DateTime SentDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the expected return date
    /// </summary>
    public DateTime? ExpectedReturnDate { get; set; }

    /// <summary>
    /// Gets or sets the actual return date
    /// </summary>
    public DateTime? ActualReturnDate { get; set; }

    /// <summary>
    /// Gets or sets the status of the outsourced coating
    /// </summary>
    [MaxLength(50)]
    public string Status { get; set; } = "Sent";

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this record was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the coating supplier
    /// </summary>
    public Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// Gets the assembly progress records for assemblies in this list
    /// </summary>
    public ICollection<AssemblyProgress> AssemblyProgresses { get; set; } = new HashSet<AssemblyProgress>();
}