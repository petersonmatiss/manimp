using Microsoft.EntityFrameworkCore;
using Manimp.Data.Contexts;
using Manimp.Shared.Models;

namespace Manimp.Services;

/// <summary>
/// Service for managing EN 1090 assembly progress tracking
/// </summary>
public class AssemblyProgressService
{
    private readonly AppDbContext _context;

    public AssemblyProgressService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all assemblies with their current progress status
    /// </summary>
    /// <returns>List of assemblies with progress information</returns>
    public async Task<List<Assembly>> GetAssembliesWithProgressAsync()
    {
        return await _context.Assemblies
            .Include(a => a.AssemblyList)
            .ThenInclude(al => al.CrmProject)
            .ThenInclude(p => p.Customer)
            .Include(a => a.StatusHistory.OrderByDescending(sh => sh.ChangedUtc).Take(5))
            .Include(a => a.QualityAssuranceRecords)
            .Include(a => a.NonComplianceReports.Where(ncr => ncr.Status != "Closed"))
            .Include(a => a.OutsourcedCoatingTrackings)
            .OrderBy(a => a.AssemblyMark)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific assembly with full progress details
    /// </summary>
    /// <param name="assemblyId">The assembly identifier</param>
    /// <returns>Assembly with complete progress information</returns>
    public async Task<Assembly?> GetAssemblyWithProgressAsync(int assemblyId)
    {
        return await _context.Assemblies
            .Include(a => a.AssemblyList)
            .ThenInclude(al => al.CrmProject)
            .Include(a => a.StatusHistory.OrderByDescending(sh => sh.ChangedUtc))
            .Include(a => a.QualityAssuranceRecords.OrderByDescending(qa => qa.PerformedUtc))
            .Include(a => a.NonComplianceReports.OrderByDescending(ncr => ncr.DetectedUtc))
            .Include(a => a.OutsourcedCoatingTrackings.OrderByDescending(oct => oct.SentDate))
            .FirstOrDefaultAsync(a => a.AssemblyId == assemblyId);
    }

    /// <summary>
    /// Updates assembly status if all requirements are met
    /// </summary>
    /// <param name="assemblyId">The assembly identifier</param>
    /// <param name="newStatus">The new status to set</param>
    /// <param name="changedBy">Who is making the change</param>
    /// <param name="notes">Optional notes about the status change</param>
    /// <returns>Result indicating success or failure with message</returns>
    public async Task<(bool Success, string Message)> UpdateAssemblyStatusAsync(
        int assemblyId, 
        AssemblyProgressStatus newStatus, 
        string changedBy, 
        string? notes = null)
    {
        var assembly = await _context.Assemblies
            .Include(a => a.QualityAssuranceRecords)
            .Include(a => a.NonComplianceReports)
            .Include(a => a.OutsourcedCoatingTrackings)
            .FirstOrDefaultAsync(a => a.AssemblyId == assemblyId);

        if (assembly == null)
        {
            return (false, "Assembly not found");
        }

        // Validate status progression
        var validationResult = ValidateStatusProgression(assembly.CurrentStatus, newStatus);
        if (!validationResult.IsValid)
        {
            return (false, validationResult.Message);
        }

        // Check QA requirements
        var qaResult = await ValidateQARequirementsAsync(assembly, newStatus);
        if (!qaResult.IsValid)
        {
            return (false, qaResult.Message);
        }

        // Check for open NCRs that might block progression
        var ncrResult = ValidateNCRRequirements(assembly, newStatus);
        if (!ncrResult.IsValid)
        {
            return (false, ncrResult.Message);
        }

        // Special handling for coating status
        if (newStatus == AssemblyProgressStatus.CoatingDone && assembly.IsCoatingOutsourced)
        {
            var coatingResult = ValidateOutsourcedCoatingComplete(assembly);
            if (!coatingResult.IsValid)
            {
                return (false, coatingResult.Message);
            }
        }

        // Record status change
        var previousStatus = assembly.CurrentStatus;
        assembly.CurrentStatus = newStatus;

        var statusHistory = new AssemblyStatusHistory
        {
            AssemblyId = assemblyId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ChangedBy = changedBy,
            Notes = notes,
            QACompleted = qaResult.IsValid
        };

        _context.AssemblyStatusHistories.Add(statusHistory);

        // Update manufacturing timestamps
        UpdateManufacturingTimestamps(assembly, newStatus);

        await _context.SaveChangesAsync();

        return (true, $"Assembly status updated from {previousStatus} to {newStatus}");
    }

    /// <summary>
    /// Adds a quality assurance record for an assembly
    /// </summary>
    /// <param name="assemblyId">The assembly identifier</param>
    /// <param name="qaType">Type of QA check</param>
    /// <param name="result">Result of the QA check</param>
    /// <param name="performedBy">Who performed the QA check</param>
    /// <param name="forStatus">Which status this QA check is for</param>
    /// <param name="findings">Detailed findings</param>
    /// <param name="correctiveActions">Any corrective actions needed</param>
    /// <param name="en1090Reference">Reference to EN 1090 standard</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<(bool Success, string Message)> AddQualityAssuranceRecordAsync(
        int assemblyId,
        QualityAssuranceType qaType,
        QAResult result,
        string performedBy,
        AssemblyProgressStatus forStatus,
        string? findings = null,
        string? correctiveActions = null,
        string? en1090Reference = null)
    {
        var assembly = await _context.Assemblies.FindAsync(assemblyId);
        if (assembly == null)
        {
            return (false, "Assembly not found");
        }

        var qaRecord = new QualityAssuranceRecord
        {
            AssemblyId = assemblyId,
            QAType = qaType,
            Result = result,
            PerformedBy = performedBy,
            ForStatus = forStatus,
            Findings = findings,
            CorrectiveActions = correctiveActions,
            EN1090Reference = en1090Reference
        };

        _context.QualityAssuranceRecords.Add(qaRecord);
        await _context.SaveChangesAsync();

        return (true, "Quality assurance record added successfully");
    }

    /// <summary>
    /// Creates a non-compliance report for an assembly
    /// </summary>
    /// <param name="assemblyId">The assembly identifier</param>
    /// <param name="ncrNumber">The NCR number</param>
    /// <param name="detectedBy">Who detected the non-compliance</param>
    /// <param name="category">Category of non-compliance</param>
    /// <param name="severity">Severity level</param>
    /// <param name="description">Detailed description</param>
    /// <param name="en1090Reference">Reference to EN 1090 standard</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<(bool Success, string Message)> CreateNonComplianceReportAsync(
        int assemblyId,
        string ncrNumber,
        string detectedBy,
        string category,
        string severity,
        string description,
        string? en1090Reference = null)
    {
        var assembly = await _context.Assemblies.FindAsync(assemblyId);
        if (assembly == null)
        {
            return (false, "Assembly not found");
        }

        // Check if NCR number already exists
        var existingNCR = await _context.NonComplianceReports
            .FirstOrDefaultAsync(ncr => ncr.NCRNumber == ncrNumber);
        if (existingNCR != null)
        {
            return (false, "NCR number already exists");
        }

        var ncr = new NonComplianceReport
        {
            AssemblyId = assemblyId,
            NCRNumber = ncrNumber,
            DetectedBy = detectedBy,
            Category = category,
            Severity = severity,
            Description = description,
            EN1090Reference = en1090Reference,
            Status = "Open"
        };

        _context.NonComplianceReports.Add(ncr);
        await _context.SaveChangesAsync();

        return (true, "Non-compliance report created successfully");
    }

    /// <summary>
    /// Creates an outsourced coating tracking record
    /// </summary>
    /// <param name="assemblyId">The assembly identifier</param>
    /// <param name="supplierId">The coating supplier identifier</param>
    /// <param name="sentBy">Who sent the assembly for coating</param>
    /// <param name="expectedReturnDate">Expected return date</param>
    /// <param name="coatingSpecification">Coating specification</param>
    /// <param name="cost">Cost of outsourced coating</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<(bool Success, string Message)> CreateOutsourcedCoatingTrackingAsync(
        int assemblyId,
        int supplierId,
        string sentBy,
        DateTime expectedReturnDate,
        string? coatingSpecification = null,
        decimal? cost = null)
    {
        var assembly = await _context.Assemblies.FindAsync(assemblyId);
        if (assembly == null)
        {
            return (false, "Assembly not found");
        }

        var supplier = await _context.Suppliers.FindAsync(supplierId);
        if (supplier == null)
        {
            return (false, "Supplier not found");
        }

        // Mark assembly as having outsourced coating
        assembly.IsCoatingOutsourced = true;

        var tracking = new OutsourcedCoatingTracking
        {
            AssemblyId = assemblyId,
            SupplierId = supplierId,
            SentDate = DateTime.UtcNow,
            ExpectedReturnDate = expectedReturnDate,
            SentBy = sentBy,
            CoatingSpecification = coatingSpecification,
            Cost = cost,
            Status = "Sent"
        };

        _context.OutsourcedCoatingTrackings.Add(tracking);
        await _context.SaveChangesAsync();

        return (true, "Outsourced coating tracking record created successfully");
    }

    /// <summary>
    /// Gets assemblies that are ready for outsourced coating
    /// </summary>
    /// <returns>List of assemblies ready for coating</returns>
    public async Task<List<Assembly>> GetAssembliesReadyForOutsourcingAsync()
    {
        return await _context.Assemblies
            .Include(a => a.AssemblyList)
            .ThenInclude(al => al.CrmProject)
            .Where(a => a.CurrentStatus == AssemblyProgressStatus.ReadyForCoating && a.IsCoatingOutsourced)
            .OrderBy(a => a.AssemblyMark)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all open non-compliance reports
    /// </summary>
    /// <returns>List of open NCRs</returns>
    public async Task<List<NonComplianceReport>> GetOpenNonComplianceReportsAsync()
    {
        return await _context.NonComplianceReports
            .Include(ncr => ncr.Assembly)
            .ThenInclude(a => a.AssemblyList)
            .ThenInclude(al => al.CrmProject)
            .Where(ncr => ncr.Status != "Closed")
            .OrderByDescending(ncr => ncr.DetectedUtc)
            .ToListAsync();
    }

    #region Private Helper Methods

    private static (bool IsValid, string Message) ValidateStatusProgression(
        AssemblyProgressStatus currentStatus, 
        AssemblyProgressStatus newStatus)
    {
        // Define valid status progressions
        var validProgressions = new Dictionary<AssemblyProgressStatus, List<AssemblyProgressStatus>>
        {
            [AssemblyProgressStatus.NotStarted] = new() { AssemblyProgressStatus.Assembled },
            [AssemblyProgressStatus.Assembled] = new() { AssemblyProgressStatus.Welded },
            [AssemblyProgressStatus.Welded] = new() { AssemblyProgressStatus.ReadyForCoating },
            [AssemblyProgressStatus.ReadyForCoating] = new() { AssemblyProgressStatus.CoatingDone },
            [AssemblyProgressStatus.CoatingDone] = new() { AssemblyProgressStatus.ReadyForDelivery },
            [AssemblyProgressStatus.ReadyForDelivery] = new() { AssemblyProgressStatus.Delivered }
        };

        if (!validProgressions.ContainsKey(currentStatus))
        {
            return (false, $"No valid progressions defined for status {currentStatus}");
        }

        if (!validProgressions[currentStatus].Contains(newStatus))
        {
            return (false, $"Invalid progression from {currentStatus} to {newStatus}");
        }

        return (true, "Valid progression");
    }

    private async Task<(bool IsValid, string Message)> ValidateQARequirementsAsync(
        Assembly assembly, 
        AssemblyProgressStatus newStatus)
    {
        // Define required QA checks for each status
        var requiredQAChecks = new Dictionary<AssemblyProgressStatus, List<QualityAssuranceType>>
        {
            [AssemblyProgressStatus.Assembled] = new() { QualityAssuranceType.VisualTesting, QualityAssuranceType.DimensionalInspection },
            [AssemblyProgressStatus.Welded] = new() { QualityAssuranceType.VisualTesting, QualityAssuranceType.WeldQualityInspection },
            [AssemblyProgressStatus.ReadyForCoating] = new() { QualityAssuranceType.VisualTesting },
            [AssemblyProgressStatus.CoatingDone] = new() { QualityAssuranceType.CoatingQualityInspection },
            [AssemblyProgressStatus.ReadyForDelivery] = new() { QualityAssuranceType.FinalInspection },
            [AssemblyProgressStatus.Delivered] = new() { QualityAssuranceType.FinalInspection }
        };

        if (!requiredQAChecks.ContainsKey(newStatus))
        {
            return (true, "No QA requirements for this status");
        }

        var requiredChecks = requiredQAChecks[newStatus];
        var existingChecks = assembly.QualityAssuranceRecords
            .Where(qa => qa.ForStatus == newStatus && qa.Result == QAResult.Pass)
            .Select(qa => qa.QAType)
            .ToList();

        var missingChecks = requiredChecks.Except(existingChecks).ToList();
        if (missingChecks.Any())
        {
            return (false, $"Missing required QA checks: {string.Join(", ", missingChecks)}");
        }

        return (true, "All QA requirements satisfied");
    }

    private static (bool IsValid, string Message) ValidateNCRRequirements(
        Assembly assembly, 
        AssemblyProgressStatus newStatus)
    {
        var openCriticalNCRs = assembly.NonComplianceReports
            .Where(ncr => ncr.Status == "Open" && ncr.Severity == "Critical")
            .ToList();

        if (openCriticalNCRs.Any())
        {
            return (false, $"Cannot progress due to {openCriticalNCRs.Count} open critical NCR(s)");
        }

        return (true, "No blocking NCRs");
    }

    private static (bool IsValid, string Message) ValidateOutsourcedCoatingComplete(Assembly assembly)
    {
        var latestCoatingTracking = assembly.OutsourcedCoatingTrackings
            .OrderByDescending(oct => oct.SentDate)
            .FirstOrDefault();

        if (latestCoatingTracking == null)
        {
            return (false, "No outsourced coating tracking record found");
        }

        if (latestCoatingTracking.Status != "Returned" && latestCoatingTracking.ActualReturnDate == null)
        {
            return (false, "Outsourced coating not yet returned");
        }

        return (true, "Outsourced coating completed");
    }

    private static void UpdateManufacturingTimestamps(Assembly assembly, AssemblyProgressStatus newStatus)
    {
        switch (newStatus)
        {
            case AssemblyProgressStatus.Assembled:
                if (assembly.ManufacturingStarted == null)
                {
                    assembly.ManufacturingStarted = DateTime.UtcNow;
                }
                break;
            case AssemblyProgressStatus.Delivered:
                assembly.ManufacturingCompleted = DateTime.UtcNow;
                assembly.ProgressPercentage = 100;
                break;
            default:
                // Update progress percentage based on status
                assembly.ProgressPercentage = newStatus switch
                {
                    AssemblyProgressStatus.NotStarted => 0,
                    AssemblyProgressStatus.Assembled => 20,
                    AssemblyProgressStatus.Welded => 40,
                    AssemblyProgressStatus.ReadyForCoating => 60,
                    AssemblyProgressStatus.CoatingDone => 80,
                    AssemblyProgressStatus.ReadyForDelivery => 90,
                    AssemblyProgressStatus.Delivered => 100,
                    _ => assembly.ProgressPercentage
                };
                break;
        }
    }

    #endregion
}