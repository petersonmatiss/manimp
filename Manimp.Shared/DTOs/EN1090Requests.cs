using Manimp.Shared.Models;

namespace Manimp.Shared.DTOs;

/// <summary>
/// Request model for updating assembly status
/// </summary>
public class UpdateStatusRequest
{
    public AssemblyProgressStatus NewStatus { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for adding QA record
/// </summary>
public class AddQARecordRequest
{
    public QualityAssuranceType QAType { get; set; }
    public QAResult Result { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public AssemblyProgressStatus ForStatus { get; set; }
    public string? Findings { get; set; }
    public string? CorrectiveActions { get; set; }
    public string? EN1090Reference { get; set; }
}

/// <summary>
/// Request model for creating NCR
/// </summary>
public class CreateNCRRequest
{
    public string NCRNumber { get; set; } = string.Empty;
    public string DetectedBy { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EN1090Reference { get; set; }
}

/// <summary>
/// Request model for creating outsourced coating tracking
/// </summary>
public class CreateOutsourcedCoatingRequest
{
    public int SupplierId { get; set; }
    public string SentBy { get; set; } = string.Empty;
    public DateTime ExpectedReturnDate { get; set; }
    public string? CoatingSpecification { get; set; }
    public decimal? Cost { get; set; }
}