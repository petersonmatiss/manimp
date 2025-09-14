using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Represents a progress report for a specific project
/// </summary>
public class ProjectProgressReport
{
    /// <summary>
    /// Gets or sets the project identifier
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project name
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the planned delivery date
    /// </summary>
    public DateTime? PlannedDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the project status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of assemblies in the project
    /// </summary>
    public int TotalAssemblies { get; set; }

    /// <summary>
    /// Gets or sets the number of assemblies completed (delivered)
    /// </summary>
    public int CompletedAssemblies { get; set; }

    /// <summary>
    /// Gets or sets the overall project progress percentage (0-100)
    /// </summary>
    public decimal ProgressPercentage => TotalAssemblies > 0 ?
        Math.Round((decimal)CompletedAssemblies / TotalAssemblies * 100, 1) : 0;

    /// <summary>
    /// Gets or sets the progress breakdown by manufacturing step
    /// </summary>
    public List<StepProgressSummary> StepProgress { get; set; } = new List<StepProgressSummary>();

    /// <summary>
    /// Gets or sets the quality check summary
    /// </summary>
    public QualityCheckSummary QualitySummary { get; set; } = new QualityCheckSummary();

    /// <summary>
    /// Gets or sets the non-compliance records summary
    /// </summary>
    public NonComplianceSummary NonComplianceSummary { get; set; } = new NonComplianceSummary();

    /// <summary>
    /// Gets or sets the outsourced coating summary
    /// </summary>
    public OutsourcedCoatingSummary CoatingSummary { get; set; } = new OutsourcedCoatingSummary();
}

/// <summary>
/// Represents progress summary for a specific manufacturing step
/// </summary>
public class StepProgressSummary
{
    /// <summary>
    /// Gets or sets the manufacturing step
    /// </summary>
    public ManufacturingStep Step { get; set; }

    /// <summary>
    /// Gets or sets the step display name
    /// </summary>
    public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of assemblies in this step
    /// </summary>
    public int AssemblyCount { get; set; }

    /// <summary>
    /// Gets or sets the percentage of total assemblies in this step
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// Gets or sets the average time spent in this step (in days)
    /// </summary>
    public decimal? AverageTimeInStep { get; set; }
}

/// <summary>
/// Represents quality check summary for a project
/// </summary>
public class QualityCheckSummary
{
    /// <summary>
    /// Gets or sets the total number of quality checks performed
    /// </summary>
    public int TotalChecks { get; set; }

    /// <summary>
    /// Gets or sets the number of passed quality checks
    /// </summary>
    public int PassedChecks { get; set; }

    /// <summary>
    /// Gets or sets the number of failed quality checks
    /// </summary>
    public int FailedChecks { get; set; }

    /// <summary>
    /// Gets or sets the number of pending quality checks
    /// </summary>
    public int PendingChecks { get; set; }

    /// <summary>
    /// Gets or sets the quality pass rate percentage
    /// </summary>
    public decimal PassRate => TotalChecks > 0 ?
        Math.Round((decimal)PassedChecks / TotalChecks * 100, 1) : 0;
}

/// <summary>
/// Represents non-compliance records summary for a project
/// </summary>
public class NonComplianceSummary
{
    /// <summary>
    /// Gets or sets the total number of NCRs
    /// </summary>
    public int TotalNCRs { get; set; }

    /// <summary>
    /// Gets or sets the number of open NCRs
    /// </summary>
    public int OpenNCRs { get; set; }

    /// <summary>
    /// Gets or sets the number of closed NCRs
    /// </summary>
    public int ClosedNCRs { get; set; }

    /// <summary>
    /// Gets or sets the number of critical NCRs
    /// </summary>
    public int CriticalNCRs { get; set; }

    /// <summary>
    /// Gets or sets the number of major NCRs
    /// </summary>
    public int MajorNCRs { get; set; }

    /// <summary>
    /// Gets or sets the number of minor NCRs
    /// </summary>
    public int MinorNCRs { get; set; }
}

/// <summary>
/// Represents outsourced coating summary for a project
/// </summary>
public class OutsourcedCoatingSummary
{
    /// <summary>
    /// Gets or sets the total number of assemblies requiring outsourced coating
    /// </summary>
    public int TotalOutsourced { get; set; }

    /// <summary>
    /// Gets or sets the number of assemblies sent for coating
    /// </summary>
    public int SentForCoating { get; set; }

    /// <summary>
    /// Gets or sets the number of assemblies returned from coating
    /// </summary>
    public int ReturnedFromCoating { get; set; }

    /// <summary>
    /// Gets or sets the number of overdue assemblies (past expected return date)
    /// </summary>
    public int OverdueAssemblies { get; set; }

    /// <summary>
    /// Gets or sets the average coating turnaround time in days
    /// </summary>
    public decimal? AverageTurnaroundDays { get; set; }
}

/// <summary>
/// Represents detailed assembly progress information for project reports
/// </summary>
public class ProjectAssemblyDetails
{
    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the assembly mark/reference
    /// </summary>
    public string AssemblyMark { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current manufacturing step
    /// </summary>
    public ManufacturingStep CurrentStep { get; set; }

    /// <summary>
    /// Gets or sets when the current step was started
    /// </summary>
    public DateTime? CurrentStepStarted { get; set; }

    /// <summary>
    /// Gets or sets the number of days in current step
    /// </summary>
    public int? DaysInCurrentStep => CurrentStepStarted.HasValue ?
        (int)(DateTime.UtcNow - CurrentStepStarted.Value).TotalDays : null;

    /// <summary>
    /// Gets or sets whether this assembly has any open NCRs
    /// </summary>
    public bool HasOpenNCRs { get; set; }

    /// <summary>
    /// Gets or sets whether this assembly is outsourced for coating
    /// </summary>
    public bool IsCoatingOutsourced { get; set; }

    /// <summary>
    /// Gets or sets whether this assembly is overdue for coating return
    /// </summary>
    public bool IsCoatingOverdue { get; set; }

    /// <summary>
    /// Gets or sets the quality check status for this assembly
    /// </summary>
    public string QualityStatus { get; set; } = string.Empty;
}