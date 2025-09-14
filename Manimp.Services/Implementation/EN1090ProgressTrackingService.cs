using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Manimp.Data.Contexts;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

/// <summary>
/// Service for managing EN 1090 compliant assembly progress tracking
/// </summary>
public class EN1090ProgressTrackingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EN1090ProgressTrackingService> _logger;

    public EN1090ProgressTrackingService(AppDbContext context, ILogger<EN1090ProgressTrackingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Assembly Progress Management

    /// <summary>
    /// Gets the progress tracking for an assembly
    /// </summary>
    public async Task<AssemblyProgress?> GetAssemblyProgressAsync(int assemblyId)
    {
        return await _context.AssemblyProgresses
            .Include(ap => ap.Assembly)
            .Include(ap => ap.QualityChecks)
            .Include(ap => ap.StepHistory.OrderBy(h => h.StepStarted))
            .FirstOrDefaultAsync(ap => ap.AssemblyId == assemblyId);
    }

    /// <summary>
    /// Initializes progress tracking for an assembly
    /// </summary>
    public async Task<AssemblyProgress> InitializeAssemblyProgressAsync(int assemblyId, string userId)
    {
        var existingProgress = await _context.AssemblyProgresses
            .FirstOrDefaultAsync(ap => ap.AssemblyId == assemblyId);

        if (existingProgress != null)
        {
            return existingProgress;
        }

        var progress = new AssemblyProgress
        {
            AssemblyId = assemblyId,
            CurrentStep = ManufacturingStep.NotStarted,
            UpdatedBy = userId,
            CurrentStepStarted = DateTime.UtcNow,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _context.AssemblyProgresses.Add(progress);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Initialized progress tracking for assembly {AssemblyId}", assemblyId);
        return progress;
    }

    /// <summary>
    /// Advances assembly to the next manufacturing step after validating quality checks
    /// </summary>
    public async Task<bool> AdvanceToNextStepAsync(int assemblyId, string userId, string? notes = null)
    {
        var progress = await GetAssemblyProgressAsync(assemblyId);
        if (progress == null)
        {
            _logger.LogWarning("No progress tracking found for assembly {AssemblyId}", assemblyId);
            return false;
        }

        var nextStep = GetNextStep(progress.CurrentStep);
        if (nextStep == null)
        {
            _logger.LogWarning("Assembly {AssemblyId} is already at final step", assemblyId);
            return false;
        }

        // Check if required quality checks are completed
        var canAdvance = await CanAdvanceToStepAsync(assemblyId, nextStep.Value);
        if (!canAdvance)
        {
            _logger.LogWarning("Cannot advance assembly {AssemblyId} to step {NextStep} - required quality checks not completed", 
                assemblyId, nextStep);
            return false;
        }

        // Create step history record for current step completion
        var stepHistory = new AssemblyProgressStepHistory
        {
            AssemblyProgressId = progress.AssemblyProgressId,
            Step = progress.CurrentStep,
            StepStarted = progress.CurrentStepStarted ?? DateTime.UtcNow,
            StepCompleted = DateTime.UtcNow,
            UpdatedBy = userId,
            Notes = notes
        };

        // Calculate duration
        if (progress.CurrentStepStarted.HasValue)
        {
            var duration = DateTime.UtcNow - progress.CurrentStepStarted.Value;
            stepHistory.DurationHours = (decimal)duration.TotalHours;
        }

        _context.AssemblyProgressStepHistories.Add(stepHistory);

        // Update progress to next step
        progress.PreviousStep = progress.CurrentStep;
        progress.CurrentStep = nextStep.Value;
        progress.CurrentStepStarted = DateTime.UtcNow;
        progress.CurrentStepCompleted = null;
        progress.UpdatedBy = userId;
        progress.UpdatedUtc = DateTime.UtcNow;
        progress.StepNotes = notes;

        // Create required quality checks for the new step
        await CreateRequiredQualityChecksAsync(progress.AssemblyProgressId, nextStep.Value);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Advanced assembly {AssemblyId} from {PreviousStep} to {CurrentStep} by user {UserId}", 
            assemblyId, progress.PreviousStep, progress.CurrentStep, userId);

        return true;
    }

    /// <summary>
    /// Marks current step as completed without advancing
    /// </summary>
    public async Task<bool> CompleteCurrentStepAsync(int assemblyId, string userId, string? notes = null)
    {
        var progress = await GetAssemblyProgressAsync(assemblyId);
        if (progress == null)
        {
            return false;
        }

        progress.CurrentStepCompleted = DateTime.UtcNow;
        progress.UpdatedBy = userId;
        progress.UpdatedUtc = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(notes))
        {
            progress.StepNotes = notes;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Completed current step {CurrentStep} for assembly {AssemblyId} by user {UserId}", 
            progress.CurrentStep, assemblyId, userId);

        return true;
    }

    /// <summary>
    /// Sets coating as outsourced and handles the coating step logic
    /// </summary>
    public async Task<bool> SetCoatingOutsourcedAsync(int assemblyId, int supplierId, DateTime expectedReturnDate, string userId)
    {
        var progress = await GetAssemblyProgressAsync(assemblyId);
        if (progress == null)
        {
            return false;
        }

        // Ensure assembly is at ReadyForCoating step
        if (progress.CurrentStep != ManufacturingStep.ReadyForCoating)
        {
            _logger.LogWarning("Cannot set coating as outsourced for assembly {AssemblyId} - not at ReadyForCoating step", assemblyId);
            return false;
        }

        progress.IsCoatingOutsourced = true;
        progress.OutsourcedCoatingSentDate = DateTime.UtcNow;
        progress.OutsourcedCoatingExpectedReturnDate = expectedReturnDate;
        progress.UpdatedBy = userId;
        progress.UpdatedUtc = DateTime.UtcNow;

        // Create outsourcing record
        var outsourcing = new AssemblyOutsourcing
        {
            AssemblyId = assemblyId,
            SupplierId = supplierId,
            Description = "Coating",
            Status = "Sent",
            SentDate = DateTime.UtcNow,
            ExpectedReturnDate = expectedReturnDate
        };

        _context.AssemblyOutsourcings.Add(outsourcing);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Set coating as outsourced for assembly {AssemblyId} to supplier {SupplierId}", 
            assemblyId, supplierId);

        return true;
    }

    /// <summary>
    /// Records return from outsourced coating
    /// </summary>
    public async Task<bool> RecordOutsourcedCoatingReturnAsync(int assemblyId, string userId, string? notes = null)
    {
        var progress = await GetAssemblyProgressAsync(assemblyId);
        if (progress == null || !progress.IsCoatingOutsourced)
        {
            return false;
        }

        progress.OutsourcedCoatingActualReturnDate = DateTime.UtcNow;
        progress.UpdatedBy = userId;
        progress.UpdatedUtc = DateTime.UtcNow;

        // Update outsourcing record
        var outsourcing = await _context.AssemblyOutsourcings
            .Where(ao => ao.AssemblyId == assemblyId && ao.Status == "Sent")
            .FirstOrDefaultAsync();

        if (outsourcing != null)
        {
            outsourcing.Status = "Returned";
            outsourcing.ActualReturnDate = DateTime.UtcNow;
            outsourcing.Notes = notes;
        }

        // Automatically advance to CoatingDone step
        await AdvanceToNextStepAsync(assemblyId, userId, notes);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Recorded return from outsourced coating for assembly {AssemblyId}", assemblyId);
        return true;
    }

    #endregion

    #region Quality Check Management

    /// <summary>
    /// Creates required quality checks for a manufacturing step
    /// </summary>
    private async Task CreateRequiredQualityChecksAsync(int assemblyProgressId, ManufacturingStep step)
    {
        var requiredChecks = GetRequiredQualityChecks(step);

        foreach (var checkType in requiredChecks)
        {
            var qualityCheck = new QualityCheck
            {
                AssemblyProgressId = assemblyProgressId,
                CheckType = checkType,
                ForStep = step,
                Status = QualityCheckStatus.Pending,
                IsRequired = true,
                CreatedUtc = DateTime.UtcNow
            };

            _context.QualityChecks.Add(qualityCheck);
        }
    }

    /// <summary>
    /// Gets the required quality check types for a manufacturing step
    /// </summary>
    private static List<QualityCheckType> GetRequiredQualityChecks(ManufacturingStep step)
    {
        return step switch
        {
            ManufacturingStep.Assembled => new List<QualityCheckType> 
            { 
                QualityCheckType.VisualTesting, 
                QualityCheckType.DimensionalCheck,
                QualityCheckType.QualityAssurance 
            },
            ManufacturingStep.Welded => new List<QualityCheckType> 
            { 
                QualityCheckType.VisualTesting, 
                QualityCheckType.WeldQualityCheck,
                QualityCheckType.QualityAssurance 
            },
            ManufacturingStep.ReadyForCoating => new List<QualityCheckType> 
            { 
                QualityCheckType.VisualTesting,
                QualityCheckType.QualityAssurance 
            },
            ManufacturingStep.CoatingDone => new List<QualityCheckType> 
            { 
                QualityCheckType.VisualTesting, 
                QualityCheckType.CoatingQualityCheck,
                QualityCheckType.QualityAssurance 
            },
            ManufacturingStep.ReadyForDelivery => new List<QualityCheckType> 
            { 
                QualityCheckType.FinalInspection,
                QualityCheckType.QualityAssurance 
            },
            _ => new List<QualityCheckType>()
        };
    }

    /// <summary>
    /// Performs a quality check
    /// </summary>
    public async Task<bool> PerformQualityCheckAsync(int qualityCheckId, QualityCheckStatus status, 
        string checkedBy, string? results = null, string? defectsFound = null, string? correctiveActions = null)
    {
        var qualityCheck = await _context.QualityChecks.FindAsync(qualityCheckId);
        if (qualityCheck == null)
        {
            return false;
        }

        qualityCheck.Status = status;
        qualityCheck.CheckedDate = DateTime.UtcNow;
        qualityCheck.CheckedBy = checkedBy;
        qualityCheck.CheckResults = results;
        qualityCheck.DefectsFound = defectsFound;
        qualityCheck.CorrectiveActions = correctiveActions;

        // If failed, create NCR
        if (status == QualityCheckStatus.Failed)
        {
            await CreateNonComplianceRecordAsync(qualityCheck, defectsFound ?? "Quality check failed");
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Quality check {QualityCheckId} completed with status {Status} by {CheckedBy}", 
            qualityCheckId, status, checkedBy);

        return true;
    }

    /// <summary>
    /// Checks if assembly can advance to the specified step
    /// </summary>
    private async Task<bool> CanAdvanceToStepAsync(int assemblyId, ManufacturingStep toStep)
    {
        var progress = await _context.AssemblyProgresses
            .Include(ap => ap.QualityChecks)
            .FirstOrDefaultAsync(ap => ap.AssemblyId == assemblyId);

        if (progress == null) return false;

        // Check if all required quality checks for current step are passed
        var requiredChecks = progress.QualityChecks
            .Where(qc => qc.ForStep == progress.CurrentStep && qc.IsRequired)
            .ToList();

        return requiredChecks.All(qc => qc.Status == QualityCheckStatus.Passed || qc.Status == QualityCheckStatus.FailedAccepted);
    }

    #endregion

    #region Non-Compliance Record Management

    /// <summary>
    /// Creates a non-compliance record
    /// </summary>
    private async Task<NonComplianceRecord> CreateNonComplianceRecordAsync(QualityCheck qualityCheck, string description)
    {
        var ncrNumber = await GenerateNCRNumberAsync();

        var ncr = new NonComplianceRecord
        {
            NCRNumber = ncrNumber,
            QualityCheckId = qualityCheck.QualityCheckId,
            AssemblyId = qualityCheck.AssemblyProgress.AssemblyId,
            DiscoveredAtStep = qualityCheck.ForStep,
            Severity = NonComplianceSeverity.Major, // Default, should be updated by user
            Status = NonComplianceStatus.Open,
            Description = description,
            DiscoveredBy = qualityCheck.CheckedBy ?? "System",
            DiscoveredDate = DateTime.UtcNow,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _context.NonComplianceRecords.Add(ncr);
        return ncr;
    }

    /// <summary>
    /// Gets all open non-compliance records
    /// </summary>
    public async Task<List<NonComplianceRecord>> GetOpenNonComplianceRecordsAsync()
    {
        return await _context.NonComplianceRecords
            .Include(ncr => ncr.Assembly)
            .Include(ncr => ncr.QualityCheck)
            .Where(ncr => ncr.Status != NonComplianceStatus.Closed && ncr.Status != NonComplianceStatus.ClosedWithConcession)
            .OrderByDescending(ncr => ncr.DiscoveredDate)
            .ToListAsync();
    }

    /// <summary>
    /// Updates a non-compliance record
    /// </summary>
    public async Task<bool> UpdateNonComplianceRecordAsync(int ncrId, NonComplianceStatus status, 
        string? rootCause = null, string? immediateAction = null, string? preventiveAction = null,
        string? assignedTo = null, DateTime? targetResolutionDate = null)
    {
        var ncr = await _context.NonComplianceRecords.FindAsync(ncrId);
        if (ncr == null) return false;

        ncr.Status = status;
        ncr.RootCause = rootCause ?? ncr.RootCause;
        ncr.ImmediateAction = immediateAction ?? ncr.ImmediateAction;
        ncr.PreventiveAction = preventiveAction ?? ncr.PreventiveAction;
        ncr.AssignedTo = assignedTo ?? ncr.AssignedTo;
        ncr.TargetResolutionDate = targetResolutionDate ?? ncr.TargetResolutionDate;
        ncr.UpdatedUtc = DateTime.UtcNow;

        if (status == NonComplianceStatus.Closed || status == NonComplianceStatus.ClosedWithConcession)
        {
            ncr.ActualResolutionDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated NCR {NCRNumber} to status {Status}", ncr.NCRNumber, status);
        return true;
    }

    /// <summary>
    /// Generates a unique NCR number
    /// </summary>
    private async Task<string> GenerateNCRNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"NCR-{year}-";
        
        var lastNcr = await _context.NonComplianceRecords
            .Where(ncr => ncr.NCRNumber.StartsWith(prefix))
            .OrderByDescending(ncr => ncr.NCRNumber)
            .FirstOrDefaultAsync();

        var nextNumber = 1;
        if (lastNcr != null)
        {
            var lastNumberPart = lastNcr.NCRNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberPart, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D4}";
    }

    #endregion

    #region Outsourced Coating List Management

    /// <summary>
    /// Gets assemblies ready for outsourced coating
    /// </summary>
    public async Task<List<AssemblyProgress>> GetAssembliesReadyForOutsourcedCoatingAsync()
    {
        return await _context.AssemblyProgresses
            .Include(ap => ap.Assembly)
            .Where(ap => ap.CurrentStep == ManufacturingStep.ReadyForCoating && 
                        !ap.IsCoatingOutsourced)
            .ToListAsync();
    }

    /// <summary>
    /// Gets assemblies currently at outsourced coating
    /// </summary>
    public async Task<List<AssemblyProgress>> GetAssembliesAtOutsourcedCoatingAsync()
    {
        return await _context.AssemblyProgresses
            .Include(ap => ap.Assembly)
            .Where(ap => ap.IsCoatingOutsourced && 
                        ap.OutsourcedCoatingActualReturnDate == null)
            .ToListAsync();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the next manufacturing step
    /// </summary>
    private static ManufacturingStep? GetNextStep(ManufacturingStep currentStep)
    {
        return currentStep switch
        {
            ManufacturingStep.NotStarted => ManufacturingStep.Assembled,
            ManufacturingStep.Assembled => ManufacturingStep.Welded,
            ManufacturingStep.Welded => ManufacturingStep.ReadyForCoating,
            ManufacturingStep.ReadyForCoating => ManufacturingStep.CoatingDone,
            ManufacturingStep.CoatingDone => ManufacturingStep.ReadyForDelivery,
            ManufacturingStep.ReadyForDelivery => ManufacturingStep.Delivered,
            ManufacturingStep.Delivered => null,
            _ => null
        };
    }

    /// <summary>
    /// Gets assemblies by current manufacturing step
    /// </summary>
    public async Task<List<AssemblyProgress>> GetAssembliesByStepAsync(ManufacturingStep step)
    {
        return await _context.AssemblyProgresses
            .Include(ap => ap.Assembly)
            .Include(ap => ap.QualityChecks.Where(qc => qc.ForStep == step))
            .Where(ap => ap.CurrentStep == step)
            .OrderBy(ap => ap.CurrentStepStarted)
            .ToListAsync();
    }

    /// <summary>
    /// Gets complete progress report for an assembly
    /// </summary>
    public async Task<AssemblyProgress?> GetCompleteProgressReportAsync(int assemblyId)
    {
        return await _context.AssemblyProgresses
            .Include(ap => ap.Assembly)
                .ThenInclude(a => a.AssemblyList)
                    .ThenInclude(al => al.CrmProject)
            .Include(ap => ap.QualityChecks.OrderBy(qc => qc.CreatedUtc))
            .Include(ap => ap.StepHistory.OrderBy(h => h.StepStarted))
            .FirstOrDefaultAsync(ap => ap.AssemblyId == assemblyId);
    }

    #endregion
}