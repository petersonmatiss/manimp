using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Manimp.Data.Contexts;
using Manimp.Shared.Models;
using Manimp.Shared.Interfaces;
using System.Text.Json;
using OfficeOpenXml;

namespace Manimp.Services.Implementation;

/// <summary>
/// Service for handling assembly list file uploads with tier-based functionality
/// </summary>
public class AssemblyListUploadService
{
    private readonly AppDbContext _context;
    private readonly IFeatureGate _featureGateService;
    private readonly ILogger<AssemblyListUploadService> _logger;

    static AssemblyListUploadService()
    {
        // Set EPPlus license context for non-commercial use
        // Note: This uses the legacy API as the new License property is read-only in EPPlus 8.1.1
        // This warning will be resolved when EPPlus provides the correct initialization method
#pragma warning disable CS0618 // Type or member is obsolete
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public AssemblyListUploadService(
        AppDbContext context,
        IFeatureGate featureGateService,
        ILogger<AssemblyListUploadService> logger)
    {
        _context = context;
        _featureGateService = featureGateService;
        _logger = logger;
    }

    /// <summary>
    /// Uploads and processes an assembly list file based on tenant tier
    /// </summary>
    public async Task<AssemblyListUploadResult> UploadAssemblyListAsync(
        Guid tenantId,
        int crmProjectId,
        IFormFile file,
        string assemblyListName,
        ColumnMapping? columnMapping = null)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyListName);

        // Check feature access based on tier
        var hasBasicUpload = await _featureGateService.IsFeatureEnabledAsync(tenantId, FeatureKeys.AssemblyListUpload);
        var hasTier2Upload = await _featureGateService.IsFeatureEnabledAsync(tenantId, FeatureKeys.AssemblyListUploadTier2);
        var hasTier3Upload = await _featureGateService.IsFeatureEnabledAsync(tenantId, FeatureKeys.AssemblyListUploadTier3);

        if (!hasBasicUpload)
        {
            return new AssemblyListUploadResult
            {
                Success = false,
                ErrorMessage = "File upload is not available for your subscription tier. Please upgrade or enter data manually."
            };
        }

        // Validate file
        if (file == null || file.Length == 0)
        {
            return new AssemblyListUploadResult
            {
                Success = false,
                ErrorMessage = "No file was uploaded."
            };
        }

        if (!IsValidExcelFile(file))
        {
            return new AssemblyListUploadResult
            {
                Success = false,
                ErrorMessage = "Only Excel files (.xlsx, .xls) are supported."
            };
        }

        try
        {
            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);

            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                return new AssemblyListUploadResult
                {
                    Success = false,
                    ErrorMessage = "The Excel file does not contain any worksheets."
                };
            }

            // Process based on tier capability
            if (hasTier3Upload)
            {
                return await ProcessWithAiParsing(crmProjectId, assemblyListName, worksheet, file.FileName);
            }
            else if (hasTier2Upload && columnMapping != null)
            {
                return await ProcessWithManualMapping(crmProjectId, assemblyListName, worksheet, columnMapping, file.FileName);
            }
            else
            {
                return new AssemblyListUploadResult
                {
                    Success = false,
                    ErrorMessage = "Manual column mapping is required for your subscription tier.",
                    RequiresColumnMapping = true,
                    DetectedColumns = DetectColumns(worksheet)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing assembly list upload for project {ProjectId}", crmProjectId);
            return new AssemblyListUploadResult
            {
                Success = false,
                ErrorMessage = "An error occurred while processing the file. Please check the format and try again."
            };
        }
    }

    /// <summary>
    /// Detects available columns in the Excel file for manual mapping
    /// </summary>
    public ColumnDetectionResult DetectColumns(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);

            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                return new ColumnDetectionResult { Success = false, ErrorMessage = "No worksheets found." };
            }

            var columns = DetectColumns(worksheet);
            return new ColumnDetectionResult
            {
                Success = true,
                Columns = columns,
                RowCount = worksheet.Dimension?.Rows ?? 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting columns in uploaded file");
            return new ColumnDetectionResult
            {
                Success = false,
                ErrorMessage = "Error reading the Excel file."
            };
        }
    }

    /// <summary>
    /// Processes file using AI-powered parsing (Tier 3)
    /// </summary>
    private async Task<AssemblyListUploadResult> ProcessWithAiParsing(
        int crmProjectId,
        string assemblyListName,
        ExcelWorksheet worksheet,
        string fileName)
    {
        // AI-powered column detection
        var detectedMapping = DetectColumnsWithAi(worksheet);

        _logger.LogInformation("AI detected column mapping for project {ProjectId}: {Mapping}",
            crmProjectId, JsonSerializer.Serialize(detectedMapping));

        return await ProcessAssemblyData(crmProjectId, assemblyListName, worksheet, detectedMapping, fileName);
    }

    /// <summary>
    /// Processes file using manual column mapping (Tier 2)
    /// </summary>
    private async Task<AssemblyListUploadResult> ProcessWithManualMapping(
        int crmProjectId,
        string assemblyListName,
        ExcelWorksheet worksheet,
        ColumnMapping columnMapping,
        string fileName)
    {
        _logger.LogInformation("Processing with manual column mapping for project {ProjectId}: {Mapping}",
            crmProjectId, JsonSerializer.Serialize(columnMapping));

        return await ProcessAssemblyData(crmProjectId, assemblyListName, worksheet, columnMapping, fileName);
    }

    /// <summary>
    /// Core method to process assembly data and save to database
    /// </summary>
    private async Task<AssemblyListUploadResult> ProcessAssemblyData(
        int crmProjectId,
        string assemblyListName,
        ExcelWorksheet worksheet,
        ColumnMapping mapping,
        string fileName)
    {
        var assemblyList = new AssemblyList
        {
            CrmProjectId = crmProjectId,
            Name = assemblyListName,
            UploadFileName = fileName,
            CreatedUtc = DateTime.UtcNow
        };

        _context.AssemblyLists.Add(assemblyList);
        await _context.SaveChangesAsync(); // Save to get the ID

        var assemblies = new List<Assembly>();
        var parts = new List<Part>();
        var assemblyParts = new List<AssemblyPart>();
        var errors = new List<string>();

        var startRow = mapping.HeaderRow + 1;
        var endRow = worksheet.Dimension?.Rows ?? 0;

        for (int row = startRow; row <= endRow; row++)
        {
            try
            {
                var assemblyMark = GetCellValue(worksheet, row, mapping.AssemblyMarkColumn);
                if (string.IsNullOrWhiteSpace(assemblyMark))
                    continue; // Skip empty rows

                var assembly = new Assembly
                {
                    AssemblyListId = assemblyList.AssemblyListId,
                    AssemblyMark = assemblyMark,
                    Description = GetCellValue(worksheet, row, mapping.DescriptionColumn),
                    Quantity = ParseInt(GetCellValue(worksheet, row, mapping.QuantityColumn)) ?? 1,
                    Weight = ParseDecimal(GetCellValue(worksheet, row, mapping.WeightColumn)),
                    CreatedUtc = DateTime.UtcNow
                };

                assemblies.Add(assembly);

                // Process parts if part information is provided
                var partNumber = GetCellValue(worksheet, row, mapping.PartNumberColumn);
                if (!string.IsNullOrWhiteSpace(partNumber))
                {
                    var part = await GetOrCreatePartAsync(partNumber, mapping, worksheet, row);
                    if (part != null)
                    {
                        // We'll link this after saving assemblies
                        assemblyParts.Add(new AssemblyPart
                        {
                            Assembly = assembly,
                            Part = part,
                            Quantity = ParseInt(GetCellValue(worksheet, row, mapping.PartQuantityColumn)) ?? 1,
                            CreatedUtc = DateTime.UtcNow
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Row {row}: {ex.Message}");
                _logger.LogWarning(ex, "Error processing row {Row} in assembly list upload", row);
            }
        }

        // Save assemblies
        _context.Assemblies.AddRange(assemblies);
        await _context.SaveChangesAsync();

        // Save assembly parts
        _context.AssemblyParts.AddRange(assemblyParts);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully imported {AssemblyCount} assemblies and {PartCount} assembly parts for project {ProjectId}",
            assemblies.Count, assemblyParts.Count, crmProjectId);

        return new AssemblyListUploadResult
        {
            Success = true,
            AssemblyListId = assemblyList.AssemblyListId,
            ImportedAssemblies = assemblies.Count,
            ImportedParts = assemblyParts.Count,
            Errors = errors
        };
    }

    /// <summary>
    /// AI-powered column detection (simulated - would integrate with actual AI service)
    /// </summary>
    private ColumnMapping DetectColumnsWithAi(ExcelWorksheet worksheet)
    {
        // This is a simplified AI simulation - in a real implementation, this would:
        // 1. Send column headers to an AI service (OpenAI, Azure Cognitive Services, etc.)
        // 2. Use pattern recognition to identify column purposes
        // 3. Return confident matches

        var columns = DetectColumns(worksheet);
        var mapping = new ColumnMapping { HeaderRow = 1 };

        // Simple heuristic-based detection (simulate AI behavior)
        foreach (var column in columns)
        {
            var header = column.Value.ToLowerInvariant();

            if (header.Contains("assembly") && header.Contains("mark") || header.Contains("assembly_mark"))
                mapping.AssemblyMarkColumn = column.Key;
            else if (header.Contains("description") || header.Contains("desc"))
                mapping.DescriptionColumn = column.Key;
            else if (header.Contains("qty") || header.Contains("quantity"))
                mapping.QuantityColumn = column.Key;
            else if (header.Contains("weight"))
                mapping.WeightColumn = column.Key;
            else if (header.Contains("part") && (header.Contains("number") || header.Contains("no")))
                mapping.PartNumberColumn = column.Key;
            else if (header.Contains("part") && header.Contains("qty"))
                mapping.PartQuantityColumn = column.Key;
            else if (header.Contains("length"))
                mapping.LengthColumn = column.Key;
            else if (header.Contains("material"))
                mapping.MaterialColumn = column.Key;
        }

        return mapping;
    }

    /// <summary>
    /// Detects column headers in the worksheet
    /// </summary>
    private Dictionary<int, string> DetectColumns(ExcelWorksheet worksheet)
    {
        var columns = new Dictionary<int, string>();
        var headerRow = 1; // Assume first row contains headers

        if (worksheet.Dimension == null)
            return columns;

        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        {
            var cellValue = worksheet.Cells[headerRow, col].Value?.ToString();
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                columns[col] = cellValue;
            }
        }

        return columns;
    }

    /// <summary>
    /// Gets or creates a part based on the data in the row
    /// </summary>
    private async Task<Part?> GetOrCreatePartAsync(string partNumber, ColumnMapping mapping, ExcelWorksheet worksheet, int row)
    {
        // Check if part already exists
        var existingPart = await _context.Parts
            .FirstOrDefaultAsync(p => p.PartNumber == partNumber);

        if (existingPart != null)
            return existingPart;

        // Create new part
        var part = new Part
        {
            PartNumber = partNumber,
            Description = GetCellValue(worksheet, row, mapping.PartDescriptionColumn),
            Length = ParseDecimal(GetCellValue(worksheet, row, mapping.LengthColumn)),
            WeightPerPiece = ParseDecimal(GetCellValue(worksheet, row, mapping.PartWeightColumn)),
            Dimensions = GetCellValue(worksheet, row, mapping.DimensionsColumn),
            IsActive = true,
            CreatedUtc = DateTime.UtcNow
        };

        _context.Parts.Add(part);
        await _context.SaveChangesAsync();

        return part;
    }

    /// <summary>
    /// Safely gets cell value as string
    /// </summary>
    private string GetCellValue(ExcelWorksheet worksheet, int row, int? col)
    {
        if (!col.HasValue || col.Value < 1) return string.Empty;

        return worksheet.Cells[row, col.Value].Value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Parses string to integer
    /// </summary>
    private int? ParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return int.TryParse(value, out var result) ? result : null;
    }

    /// <summary>
    /// Parses string to decimal
    /// </summary>
    private decimal? ParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return decimal.TryParse(value, out var result) ? result : null;
    }

    /// <summary>
    /// Validates if the uploaded file is a valid Excel file
    /// </summary>
    private bool IsValidExcelFile(IFormFile file)
    {
        var allowedExtensions = new[] { ".xlsx", ".xls" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}

/// <summary>
/// Column mapping configuration for assembly list imports
/// </summary>
public class ColumnMapping
{
    public int HeaderRow { get; set; } = 1;
    public int? AssemblyMarkColumn { get; set; }
    public int? DescriptionColumn { get; set; }
    public int? QuantityColumn { get; set; }
    public int? WeightColumn { get; set; }
    public int? PartNumberColumn { get; set; }
    public int? PartDescriptionColumn { get; set; }
    public int? PartQuantityColumn { get; set; }
    public int? LengthColumn { get; set; }
    public int? PartWeightColumn { get; set; }
    public int? DimensionsColumn { get; set; }
    public int? MaterialColumn { get; set; }
}

/// <summary>
/// Result of assembly list upload operation
/// </summary>
public class AssemblyListUploadResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? AssemblyListId { get; set; }
    public int ImportedAssemblies { get; set; }
    public int ImportedParts { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool RequiresColumnMapping { get; set; }
    public Dictionary<int, string>? DetectedColumns { get; set; }
}

/// <summary>
/// Result of column detection operation
/// </summary>
public class ColumnDetectionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<int, string> Columns { get; set; } = new();
    public int RowCount { get; set; }
}