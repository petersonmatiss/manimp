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

    #region Project Progress Reporting

    /// <summary>
    /// Gets progress reports for all projects
    /// </summary>
    public async Task<List<ProjectProgressReport>> GetAllProjectProgressReportsAsync()
    {
        var projects = await _context.CrmProjects
            .Include(p => p.Customer)
            .Include(p => p.AssemblyLists)
                .ThenInclude(al => al.Assemblies)
                    .ThenInclude(a => a.Progress)
                        .ThenInclude(ap => ap.QualityChecks)
            .Include(p => p.AssemblyLists)
                .ThenInclude(al => al.Assemblies)
                    .ThenInclude(a => a.NonComplianceRecords)
            .Where(p => p.IsActive)
            .ToListAsync();

        var reports = new List<ProjectProgressReport>();

        foreach (var project in projects)
        {
            var report = BuildProjectProgressReport(project);
            reports.Add(report);
        }

        return reports.OrderBy(r => r.ProjectName).ToList();
    }

    /// <summary>
    /// Gets progress report for a specific project
    /// </summary>
    public async Task<ProjectProgressReport?> GetProjectProgressReportAsync(int projectId)
    {
        var project = await _context.CrmProjects
            .Include(p => p.Customer)
            .Include(p => p.AssemblyLists)
                .ThenInclude(al => al.Assemblies)
                    .ThenInclude(a => a.Progress)
                        .ThenInclude(ap => ap.QualityChecks)
            .Include(p => p.AssemblyLists)
                .ThenInclude(al => al.Assemblies)
                    .ThenInclude(a => a.NonComplianceRecords)
            .FirstOrDefaultAsync(p => p.CrmProjectId == projectId);

        if (project == null) return null;

        return BuildProjectProgressReport(project);
    }

    /// <summary>
    /// Gets detailed assembly information for a project
    /// </summary>
    public async Task<List<ProjectAssemblyDetails>> GetProjectAssemblyDetailsAsync(int projectId)
    {
        var assemblies = await _context.Assemblies
            .Include(a => a.Progress)
            .Include(a => a.NonComplianceRecords.Where(ncr => ncr.Status == NonComplianceStatus.Open))
            .Where(a => a.AssemblyList.CrmProjectId == projectId)
            .ToListAsync();

        var details = new List<ProjectAssemblyDetails>();

        foreach (var assembly in assemblies)
        {
            var detail = new ProjectAssemblyDetails
            {
                AssemblyId = assembly.AssemblyId,
                AssemblyMark = assembly.AssemblyMark,
                CurrentStep = assembly.Progress?.CurrentStep ?? ManufacturingStep.NotStarted,
                CurrentStepStarted = assembly.Progress?.CurrentStepStarted,
                HasOpenNCRs = assembly.NonComplianceRecords.Any(),
                IsCoatingOutsourced = assembly.Progress?.IsCoatingOutsourced ?? false,
                IsCoatingOverdue = IsCoatingOverdue(assembly.Progress),
                QualityStatus = GetQualityStatus(assembly.Progress)
            };

            details.Add(detail);
        }

        return details.OrderBy(d => d.AssemblyMark).ToList();
    }

    /// <summary>
    /// Builds a comprehensive progress report for a project
    /// </summary>
    private ProjectProgressReport BuildProjectProgressReport(CrmProject project)
    {
        var allAssemblies = project.AssemblyLists.SelectMany(al => al.Assemblies).ToList();
        var assembliesWithProgress = allAssemblies.Where(a => a.Progress != null).ToList();

        var report = new ProjectProgressReport
        {
            ProjectId = project.CrmProjectId,
            ProjectName = project.Name,
            CustomerName = project.Customer.CompanyName,
            StartDate = project.StartDate,
            PlannedDeliveryDate = project.PlannedDeliveryDate,
            Status = project.Status,
            TotalAssemblies = allAssemblies.Count,
            CompletedAssemblies = assembliesWithProgress.Count(a => 
                a.Progress!.CurrentStep == ManufacturingStep.Delivered)
        };

        // Build step progress summary
        report.StepProgress = BuildStepProgressSummary(assembliesWithProgress, report.TotalAssemblies);

        // Build quality summary
        report.QualitySummary = BuildQualitySummary(assembliesWithProgress);

        // Build non-compliance summary
        report.NonComplianceSummary = BuildNonComplianceSummary(allAssemblies);

        // Build outsourced coating summary
        report.CoatingSummary = BuildOutsourcedCoatingSummary(assembliesWithProgress);

        return report;
    }

    /// <summary>
    /// Builds step progress summary for a project
    /// </summary>
    private List<StepProgressSummary> BuildStepProgressSummary(List<Assembly> assembliesWithProgress, int totalAssemblies)
    {
        var stepCounts = assembliesWithProgress
            .Where(a => a.Progress != null)
            .GroupBy(a => a.Progress!.CurrentStep)
            .ToDictionary(g => g.Key, g => g.Count());

        var stepProgress = new List<StepProgressSummary>();

        foreach (ManufacturingStep step in Enum.GetValues<ManufacturingStep>())
        {
            if (step == ManufacturingStep.NotStarted) continue;

            var count = stepCounts.GetValueOrDefault(step, 0);
            var percentage = totalAssemblies > 0 ? Math.Round((decimal)count / totalAssemblies * 100, 1) : 0;

            // Calculate average time in step
            var averageTime = CalculateAverageTimeInStep(assembliesWithProgress, step);

            stepProgress.Add(new StepProgressSummary
            {
                Step = step,
                StepName = GetStepDisplayName(step),
                AssemblyCount = count,
                Percentage = percentage,
                AverageTimeInStep = averageTime
            });
        }

        return stepProgress;
    }

    /// <summary>
    /// Builds quality check summary for a project
    /// </summary>
    private QualityCheckSummary BuildQualitySummary(List<Assembly> assembliesWithProgress)
    {
        var allQualityChecks = assembliesWithProgress
            .Where(a => a.Progress != null)
            .SelectMany(a => a.Progress!.QualityChecks)
            .ToList();

        return new QualityCheckSummary
        {
            TotalChecks = allQualityChecks.Count,
            PassedChecks = allQualityChecks.Count(qc => qc.Status == QualityCheckStatus.Passed),
            FailedChecks = allQualityChecks.Count(qc => qc.Status == QualityCheckStatus.Failed),
            PendingChecks = allQualityChecks.Count(qc => qc.Status == QualityCheckStatus.Pending)
        };
    }

    /// <summary>
    /// Builds non-compliance summary for a project
    /// </summary>
    private NonComplianceSummary BuildNonComplianceSummary(List<Assembly> allAssemblies)
    {
        var allNCRs = allAssemblies.SelectMany(a => a.NonComplianceRecords).ToList();

        return new NonComplianceSummary
        {
            TotalNCRs = allNCRs.Count,
            OpenNCRs = allNCRs.Count(ncr => ncr.Status == NonComplianceStatus.Open || 
                                           ncr.Status == NonComplianceStatus.UnderReview || 
                                           ncr.Status == NonComplianceStatus.CorrectiveActionInProgress ||
                                           ncr.Status == NonComplianceStatus.AwaitingVerification),
            ClosedNCRs = allNCRs.Count(ncr => ncr.Status == NonComplianceStatus.Closed || 
                                             ncr.Status == NonComplianceStatus.ClosedWithConcession),
            CriticalNCRs = allNCRs.Count(ncr => ncr.Severity == NonComplianceSeverity.Critical),
            MajorNCRs = allNCRs.Count(ncr => ncr.Severity == NonComplianceSeverity.Major),
            MinorNCRs = allNCRs.Count(ncr => ncr.Severity == NonComplianceSeverity.Minor)
        };
    }

    /// <summary>
    /// Builds outsourced coating summary for a project
    /// </summary>
    private OutsourcedCoatingSummary BuildOutsourcedCoatingSummary(List<Assembly> assembliesWithProgress)
    {
        var outsourcedAssemblies = assembliesWithProgress
            .Where(a => a.Progress?.IsCoatingOutsourced == true)
            .ToList();

        var sentForCoating = outsourcedAssemblies
            .Count(a => a.Progress!.OutsourcedCoatingSentDate.HasValue);

        var returnedFromCoating = outsourcedAssemblies
            .Count(a => a.Progress!.OutsourcedCoatingActualReturnDate.HasValue);

        var overdueAssemblies = outsourcedAssemblies
            .Count(a => IsCoatingOverdue(a.Progress));

        // Calculate average turnaround time for completed coating
        var completedCoatingJobs = outsourcedAssemblies
            .Where(a => a.Progress!.OutsourcedCoatingSentDate.HasValue && 
                       a.Progress.OutsourcedCoatingActualReturnDate.HasValue)
            .ToList();

        decimal? averageTurnaround = null;
        if (completedCoatingJobs.Any())
        {
            var totalDays = completedCoatingJobs
                .Sum(a => (a.Progress!.OutsourcedCoatingActualReturnDate!.Value - 
                          a.Progress.OutsourcedCoatingSentDate!.Value).TotalDays);
            averageTurnaround = Math.Round((decimal)(totalDays / completedCoatingJobs.Count), 1);
        }

        return new OutsourcedCoatingSummary
        {
            TotalOutsourced = outsourcedAssemblies.Count,
            SentForCoating = sentForCoating,
            ReturnedFromCoating = returnedFromCoating,
            OverdueAssemblies = overdueAssemblies,
            AverageTurnaroundDays = averageTurnaround
        };
    }

    /// <summary>
    /// Calculates average time in step for assemblies
    /// </summary>
    private decimal? CalculateAverageTimeInStep(List<Assembly> assembliesWithProgress, ManufacturingStep step)
    {
        var assembliesInStep = assembliesWithProgress
            .Where(a => a.Progress?.CurrentStep == step && a.Progress.CurrentStepStarted.HasValue)
            .ToList();

        if (!assembliesInStep.Any()) return null;

        var totalDays = assembliesInStep
            .Sum(a => (DateTime.UtcNow - a.Progress!.CurrentStepStarted!.Value).TotalDays);

        return Math.Round((decimal)(totalDays / assembliesInStep.Count), 1);
    }

    /// <summary>
    /// Checks if coating is overdue for an assembly
    /// </summary>
    private bool IsCoatingOverdue(AssemblyProgress? progress)
    {
        if (progress?.IsCoatingOutsourced != true || 
            !progress.OutsourcedCoatingExpectedReturnDate.HasValue ||
            progress.OutsourcedCoatingActualReturnDate.HasValue)
        {
            return false;
        }

        return DateTime.UtcNow > progress.OutsourcedCoatingExpectedReturnDate.Value;
    }

    /// <summary>
    /// Gets quality status description for an assembly
    /// </summary>
    private string GetQualityStatus(AssemblyProgress? progress)
    {
        if (progress?.QualityChecks == null || !progress.QualityChecks.Any())
        {
            return "No checks";
        }

        var relevantChecks = progress.QualityChecks
            .Where(qc => qc.ForStep == progress.CurrentStep)
            .ToList();

        if (!relevantChecks.Any()) return "No checks for step";

        var failedChecks = relevantChecks.Count(qc => qc.Status == QualityCheckStatus.Failed);
        var pendingChecks = relevantChecks.Count(qc => qc.Status == QualityCheckStatus.Pending);
        var passedChecks = relevantChecks.Count(qc => qc.Status == QualityCheckStatus.Passed);

        if (failedChecks > 0) return $"{failedChecks} failed";
        if (pendingChecks > 0) return $"{pendingChecks} pending";
        return "All passed";
    }

    /// <summary>
    /// Gets display name for manufacturing step
    /// </summary>
    private string GetStepDisplayName(ManufacturingStep step)
    {
        return step switch
        {
            ManufacturingStep.NotStarted => "Not Started",
            ManufacturingStep.Assembled => "Assembled",
            ManufacturingStep.Welded => "Welded",
            ManufacturingStep.ReadyForCoating => "Ready for Coating",
            ManufacturingStep.CoatingDone => "Coating Done",
            ManufacturingStep.ReadyForDelivery => "Ready for Delivery",
            ManufacturingStep.Delivered => "Delivered",
            _ => step.ToString()
        };
    }

    #endregion
}