using Microsoft.AspNetCore.Mvc;
using Manimp.Services;
using Manimp.Shared.Models;
using Manimp.Shared.DTOs;

namespace Manimp.Api.Controllers;

/// <summary>
/// API controller for EN 1090 assembly progress tracking
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AssemblyProgressController : ControllerBase
{
    private readonly AssemblyProgressService _assemblyProgressService;
    private readonly ILogger<AssemblyProgressController> _logger;

    public AssemblyProgressController(
        AssemblyProgressService assemblyProgressService,
        ILogger<AssemblyProgressController> logger)
    {
        _assemblyProgressService = assemblyProgressService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all assemblies with their progress status
    /// </summary>
    /// <returns>List of assemblies with progress information</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Assembly>>> GetAssembliesWithProgress()
    {
        try
        {
            var assemblies = await _assemblyProgressService.GetAssembliesWithProgressAsync();
            return Ok(assemblies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assemblies with progress");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets a specific assembly with full progress details
    /// </summary>
    /// <param name="id">The assembly identifier</param>
    /// <returns>Assembly with complete progress information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Assembly>> GetAssemblyWithProgress(int id)
    {
        try
        {
            var assembly = await _assemblyProgressService.GetAssemblyWithProgressAsync(id);
            if (assembly == null)
            {
                return NotFound($"Assembly with ID {id} not found");
            }
            return Ok(assembly);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assembly {AssemblyId} with progress", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Updates the status of an assembly
    /// </summary>
    /// <param name="id">The assembly identifier</param>
    /// <param name="request">The status update request</param>
    /// <returns>Result of the status update</returns>
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateAssemblyStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var result = await _assemblyProgressService.UpdateAssemblyStatusAsync(
                id, request.NewStatus, request.ChangedBy, request.Notes);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for assembly {AssemblyId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Adds a quality assurance record for an assembly
    /// </summary>
    /// <param name="id">The assembly identifier</param>
    /// <param name="request">The QA record request</param>
    /// <returns>Result of adding the QA record</returns>
    [HttpPost("{id}/qa")]
    public async Task<ActionResult> AddQualityAssuranceRecord(int id, [FromBody] AddQARecordRequest request)
    {
        try
        {
            var result = await _assemblyProgressService.AddQualityAssuranceRecordAsync(
                id, request.QAType, request.Result, request.PerformedBy, 
                request.ForStatus, request.Findings, request.CorrectiveActions, 
                request.EN1090Reference);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding QA record for assembly {AssemblyId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Creates a non-compliance report for an assembly
    /// </summary>
    /// <param name="id">The assembly identifier</param>
    /// <param name="request">The NCR creation request</param>
    /// <returns>Result of creating the NCR</returns>
    [HttpPost("{id}/ncr")]
    public async Task<ActionResult> CreateNonComplianceReport(int id, [FromBody] CreateNCRRequest request)
    {
        try
        {
            var result = await _assemblyProgressService.CreateNonComplianceReportAsync(
                id, request.NCRNumber, request.DetectedBy, request.Category, 
                request.Severity, request.Description, request.EN1090Reference);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating NCR for assembly {AssemblyId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Creates outsourced coating tracking for an assembly
    /// </summary>
    /// <param name="id">The assembly identifier</param>
    /// <param name="request">The outsourced coating request</param>
    /// <returns>Result of creating the tracking record</returns>
    [HttpPost("{id}/outsourced-coating")]
    public async Task<ActionResult> CreateOutsourcedCoatingTracking(int id, [FromBody] CreateOutsourcedCoatingRequest request)
    {
        try
        {
            var result = await _assemblyProgressService.CreateOutsourcedCoatingTrackingAsync(
                id, request.SupplierId, request.SentBy, request.ExpectedReturnDate, 
                request.CoatingSpecification, request.Cost);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating outsourced coating tracking for assembly {AssemblyId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets assemblies ready for outsourced coating
    /// </summary>
    /// <returns>List of assemblies ready for outsourcing</returns>
    [HttpGet("ready-for-outsourcing")]
    public async Task<ActionResult<IEnumerable<Assembly>>> GetAssembliesReadyForOutsourcing()
    {
        try
        {
            var assemblies = await _assemblyProgressService.GetAssembliesReadyForOutsourcingAsync();
            return Ok(assemblies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assemblies ready for outsourcing");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets all open non-compliance reports
    /// </summary>
    /// <returns>List of open NCRs</returns>
    [HttpGet("open-ncrs")]
    public async Task<ActionResult<IEnumerable<NonComplianceReport>>> GetOpenNonComplianceReports()
    {
        try
        {
            var ncrs = await _assemblyProgressService.GetOpenNonComplianceReportsAsync();
            return Ok(ncrs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving open NCRs");
            return StatusCode(500, "Internal server error");
        }
    }
}