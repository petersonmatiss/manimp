using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Manimp.Services.Implementation;
using Manimp.Shared.Models;
using System.Security.Claims;

namespace Manimp.Api.Controllers;

/// <summary>
/// API controller for EN 1090 progress tracking operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EN1090ProgressController : ControllerBase
{
    private readonly EN1090ProgressTrackingService _progressService;
    private readonly ILogger<EN1090ProgressController> _logger;

    public EN1090ProgressController(
        EN1090ProgressTrackingService progressService,
        ILogger<EN1090ProgressController> logger)
    {
        _progressService = progressService;
        _logger = logger;
    }

    /// <summary>
    /// Gets progress tracking for a specific assembly
    /// </summary>
    [HttpGet("assembly/{assemblyId}")]
    public async Task<ActionResult<AssemblyProgress>> GetAssemblyProgress(int assemblyId)
    {
        try
        {
            var progress = await _progressService.GetAssemblyProgressAsync(assemblyId);
            if (progress == null)
            {
                return NotFound($"No progress tracking found for assembly {assemblyId}");
            }

            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting progress for assembly {AssemblyId}", assemblyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets complete progress report for an assembly
    /// </summary>
    [HttpGet("assembly/{assemblyId}/report")]
    public async Task<ActionResult<AssemblyProgress>> GetCompleteProgressReport(int assemblyId)
    {
        try
        {
            var report = await _progressService.GetCompleteProgressReportAsync(assemblyId);
            if (report == null)
            {
                return NotFound($"No progress tracking found for assembly {assemblyId}");
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting complete progress report for assembly {AssemblyId}", assemblyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Initializes progress tracking for an assembly
    /// </summary>
    [HttpPost("assembly/{assemblyId}/initialize")]
    public async Task<ActionResult<AssemblyProgress>> InitializeAssemblyProgress(int assemblyId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var progress = await _progressService.InitializeAssemblyProgressAsync(assemblyId, userId);
            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing progress for assembly {AssemblyId}", assemblyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Advances assembly to the next manufacturing step
    /// </summary>
    [HttpPost("assembly/{assemblyId}/advance")]
    public async Task<IActionResult> AdvanceToNextStep(int assemblyId, [FromBody] AdvanceStepRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _progressService.AdvanceToNextStepAsync(assemblyId, userId, request.Notes);

            if (!success)
            {
                return BadRequest("Cannot advance to next step. Check that required quality checks are completed.");
            }

            return Ok(new { message = "Successfully advanced to next step" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error advancing assembly {AssemblyId} to next step", assemblyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Completes the current manufacturing step
    /// </summary>
    [HttpPost("assembly/{assemblyId}/complete-step")]
    public async Task<IActionResult> CompleteCurrentStep(int assemblyId, [FromBody] CompleteStepRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _progressService.CompleteCurrentStepAsync(assemblyId, userId, request.Notes);

            if (!success)
            {
                return BadRequest("Failed to complete current step");
            }

            return Ok(new { message = "Successfully completed current step" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing current step for assembly {AssemblyId}", assemblyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Sets coating as outsourced for an assembly
    /// </summary>
    [HttpPost("assembly/{assemblyId}/outsource-coating")]
    public async Task<IActionResult> SetCoatingOutsourced(int assemblyId, [FromBody] OutsourceCoatingRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _progressService.SetCoatingOutsourcedAsync(
                assemblyId, request.SupplierId, request.ExpectedReturnDate, userId);

            if (!success)
            {
                return BadRequest("Failed to set coating as outsourced. Ensure assembly is at ReadyForCoating step.");
            }

            return Ok(new { message = "Successfully set coating as outsourced" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting coating as outsourced for assembly {AssemblyId}", assemblyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Records return from outsourced coating
    /// </summary>
    [HttpPost("assembly/{assemblyId}/coating-returned")]
    public async Task<IActionResult> RecordCoatingReturn(int assemblyId, [FromBody] CoatingReturnRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _progressService.RecordOutsourcedCoatingReturnAsync(assemblyId, userId, request.Notes);

            if (!success)
            {
                return BadRequest("Failed to record coating return");
            }

            return Ok(new { message = "Successfully recorded coating return" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording coating return for assembly {AssemblyId}", assemblyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets assemblies by manufacturing step
    /// </summary>
    [HttpGet("assemblies/step/{step}")]
    public async Task<ActionResult<List<AssemblyProgress>>> GetAssembliesByStep(ManufacturingStep step)
    {
        try
        {
            var assemblies = await _progressService.GetAssembliesByStepAsync(step);
            return Ok(assemblies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assemblies by step {Step}", step);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets assemblies ready for outsourced coating
    /// </summary>
    [HttpGet("assemblies/ready-for-outsourced-coating")]
    public async Task<ActionResult<List<AssemblyProgress>>> GetAssembliesReadyForOutsourcedCoating()
    {
        try
        {
            var assemblies = await _progressService.GetAssembliesReadyForOutsourcedCoatingAsync();
            return Ok(assemblies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assemblies ready for outsourced coating");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets assemblies currently at outsourced coating
    /// </summary>
    [HttpGet("assemblies/at-outsourced-coating")]
    public async Task<ActionResult<List<AssemblyProgress>>> GetAssembliesAtOutsourcedCoating()
    {
        try
        {
            var assemblies = await _progressService.GetAssembliesAtOutsourcedCoatingAsync();
            return Ok(assemblies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assemblies at outsourced coating");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Performs a quality check
    /// </summary>
    [HttpPost("quality-check/{qualityCheckId}")]
    public async Task<IActionResult> PerformQualityCheck(int qualityCheckId, [FromBody] QualityCheckRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _progressService.PerformQualityCheckAsync(
                qualityCheckId, request.Status, userId, request.Results, request.DefectsFound, request.CorrectiveActions);

            if (!success)
            {
                return BadRequest("Failed to perform quality check");
            }

            return Ok(new { message = "Quality check completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing quality check {QualityCheckId}", qualityCheckId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets all open non-compliance records
    /// </summary>
    [HttpGet("ncr/open")]
    public async Task<ActionResult<List<NonComplianceRecord>>> GetOpenNonComplianceRecords()
    {
        try
        {
            var ncrs = await _progressService.GetOpenNonComplianceRecordsAsync();
            return Ok(ncrs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting open non-compliance records");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Updates a non-compliance record
    /// </summary>
    [HttpPut("ncr/{ncrId}")]
    public async Task<IActionResult> UpdateNonComplianceRecord(int ncrId, [FromBody] UpdateNCRRequest request)
    {
        try
        {
            var success = await _progressService.UpdateNonComplianceRecordAsync(
                ncrId, request.Status, request.RootCause, request.ImmediateAction,
                request.PreventiveAction, request.AssignedTo, request.TargetResolutionDate);

            if (!success)
            {
                return NotFound($"Non-compliance record {ncrId} not found");
            }

            return Ok(new { message = "Non-compliance record updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating non-compliance record {NCRId}", ncrId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets the current user ID from the JWT token
    /// </summary>
    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               User.FindFirst("sub")?.Value ??
               User.Identity?.Name ??
               "Unknown";
    }

    /// <summary>
    /// Gets progress reports for all projects
    /// </summary>
    [HttpGet("projects")]
    public async Task<ActionResult<List<ProjectProgressReport>>> GetAllProjectProgressReports()
    {
        try
        {
            var reports = await _progressService.GetAllProjectProgressReportsAsync();
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project progress reports");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets progress report for a specific project
    /// </summary>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<ProjectProgressReport>> GetProjectProgressReport(int projectId)
    {
        try
        {
            var report = await _progressService.GetProjectProgressReportAsync(projectId);
            if (report == null)
            {
                return NotFound($"No project found with ID {projectId}");
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting progress report for project {ProjectId}", projectId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets detailed assembly information for a project
    /// </summary>
    [HttpGet("project/{projectId}/assemblies")]
    public async Task<ActionResult<List<ProjectAssemblyDetails>>> GetProjectAssemblyDetails(int projectId)
    {
        try
        {
            var details = await _progressService.GetProjectAssemblyDetailsAsync(projectId);
            return Ok(details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assembly details for project {ProjectId}", projectId);
            return StatusCode(500, "Internal server error");
        }
    }
}

/// <summary>
/// Request model for advancing to next step
/// </summary>
public class AdvanceStepRequest
{
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for completing current step
/// </summary>
public class CompleteStepRequest
{
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for outsourcing coating
/// </summary>
public class OutsourceCoatingRequest
{
    public int SupplierId { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
}

/// <summary>
/// Request model for coating return
/// </summary>
public class CoatingReturnRequest
{
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for quality check
/// </summary>
public class QualityCheckRequest
{
    public QualityCheckStatus Status { get; set; }
    public string? Results { get; set; }
    public string? DefectsFound { get; set; }
    public string? CorrectiveActions { get; set; }
}

/// <summary>
/// Request model for updating NCR
/// </summary>
public class UpdateNCRRequest
{
    public NonComplianceStatus Status { get; set; }
    public string? RootCause { get; set; }
    public string? ImmediateAction { get; set; }
    public string? PreventiveAction { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? TargetResolutionDate { get; set; }
}