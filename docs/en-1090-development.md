# EN 1090 Software Development Requirements

This document provides specific technical requirements and implementation guidelines for integrating EN 1090 compliance into the Manimp application.

## Table of Contents

1. [Development Priorities](#development-priorities)
2. [Database Schema Requirements](#database-schema-requirements)
3. [API Specifications](#api-specifications)
4. [User Interface Requirements](#user-interface-requirements)
5. [Workflow Integration](#workflow-integration)
6. [Testing and Validation](#testing-and-validation)
7. [Deployment Considerations](#deployment-considerations)

---

## Development Priorities

### High Priority (Phase 1 - Next 3 months)

#### 1. Execution Class Determination Engine
**Business Value:** Core compliance requirement that affects all other features
**Technical Complexity:** Medium
**Dependencies:** None

**Requirements:**
- Rule-based engine for automatic execution class assignment
- Configuration UI for project parameters
- Integration with existing project management module

#### 2. Material Certificate Management
**Business Value:** High - Required for material traceability
**Technical Complexity:** Low-Medium
**Dependencies:** Existing inventory system

**Requirements:**
- Digital certificate storage and retrieval
- EN 10204 certificate type validation
- Heat/batch number tracking enhancement

#### 3. Basic Compliance Dashboard
**Business Value:** High - Visibility into compliance status
**Technical Complexity:** Low
**Dependencies:** Execution class engine

**Requirements:**
- Project compliance status overview
- Outstanding requirement tracking
- Compliance progress indicators

### Medium Priority (Phase 2 - 3-6 months)

#### 4. Welding Management System
**Business Value:** High for fabrication companies
**Technical Complexity:** High
**Dependencies:** Material management, project configuration

#### 5. Inspection Planning Module
**Business Value:** Medium-High
**Technical Complexity:** Medium
**Dependencies:** Execution class engine, project management

### Lower Priority (Phase 3 - 6+ months)

#### 6. Automated Document Generation
**Business Value:** High long-term value
**Technical Complexity:** High
**Dependencies:** All other modules

---

## Database Schema Requirements

### New Tables for EN 1090 Compliance

```sql
-- Execution Class Configuration
CREATE TABLE ExecutionClasses (
    ExecutionClassId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId UNIQUEIDENTIFIER NOT NULL,
    ClassLevel NVARCHAR(10) NOT NULL, -- EXC1, EXC2, EXC3, EXC4
    ConsequenceClass NVARCHAR(10), -- CC1, CC2, CC3
    SeismicCategory NVARCHAR(20),
    ServiceCategory NVARCHAR(20),
    DesignWorkingLife INT,
    EnvironmentalExposure NVARCHAR(50),
    DeterminationDate DATETIME2 DEFAULT GETUTCDATE(),
    DeterminedByUserId NVARCHAR(450),
    Justification NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId),
    FOREIGN KEY (DeterminedByUserId) REFERENCES AspNetUsers(Id)
);

-- Material Certificates Enhancement
CREATE TABLE MaterialCertificates (
    CertificateId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProfileInventoryId UNIQUEIDENTIFIER NOT NULL,
    CertificateType NVARCHAR(20) NOT NULL, -- EN10204_2_1, EN10204_3_1, EN10204_3_2
    CertificateNumber NVARCHAR(100),
    HeatNumber NVARCHAR(100),
    CastNumber NVARCHAR(100),
    ManufacturerName NVARCHAR(200),
    ManufacturerAddress NVARCHAR(500),
    ProductStandard NVARCHAR(100), -- EN 10025-2, EN 10210-1, etc.
    SteelGrade NVARCHAR(50),
    
    -- Chemical Composition (JSON format)
    ChemicalComposition NVARCHAR(MAX),
    
    -- Mechanical Properties (JSON format)
    MechanicalProperties NVARCHAR(MAX),
    
    -- Certificate Document
    CertificateDocument VARBINARY(MAX),
    CertificateFileName NVARCHAR(255),
    CertificateMimeType NVARCHAR(100),
    
    -- Dates
    CertificateDate DATETIME2,
    ValidityDate DATETIME2,
    UploadedDate DATETIME2 DEFAULT GETUTCDATE(),
    UploadedByUserId NVARCHAR(450),
    
    -- Verification
    IsVerified BIT DEFAULT 0,
    VerifiedDate DATETIME2,
    VerifiedByUserId NVARCHAR(450),
    VerificationNotes NVARCHAR(MAX),
    
    CreatedUtc DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedUtc DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (ProfileInventoryId) REFERENCES ProfileInventory(Id),
    FOREIGN KEY (UploadedByUserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (VerifiedByUserId) REFERENCES AspNetUsers(Id)
);

-- Welding Procedures
CREATE TABLE WeldingProcedures (
    WPSId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WPSNumber NVARCHAR(50) UNIQUE NOT NULL,
    Title NVARCHAR(200),
    
    -- Base Materials
    BaseMetalGroup NVARCHAR(50),
    BaseMetalType1 NVARCHAR(100),
    BaseMetalType2 NVARCHAR(100),
    ThicknessRange NVARCHAR(50), -- "6mm to 25mm"
    
    -- Welding Process
    WeldingProcess NVARCHAR(100), -- "Manual Metal Arc (111)", "Gas Metal Arc (135)"
    WeldingPosition NVARCHAR(100), -- "PA, PB, PC, PD, PE, PF"
    
    -- Consumables
    FillerMetalType NVARCHAR(100),
    FillerMetalSize NVARCHAR(50),
    ShieldingGas NVARCHAR(100),
    GasFlowRate NVARCHAR(50),
    
    -- Thermal Conditions
    PreHeatTemperature NVARCHAR(50),
    InterpassTemperature NVARCHAR(50),
    PostWeldHeatTreatment NVARCHAR(200),
    
    -- Welding Technique
    WeldingTechnique NVARCHAR(MAX), -- JSON with detailed parameters
    
    -- Qualification
    QualificationDate DATETIME2,
    QualificationStandard NVARCHAR(100), -- "EN ISO 15614-1:2017"
    WPQRReference NVARCHAR(100),
    QualifiedByOrganization NVARCHAR(200),
    
    -- Validity
    IsActive BIT DEFAULT 1,
    ExpiryDate DATETIME2,
    
    -- Metadata
    CreatedByUserId NVARCHAR(450),
    CreatedUtc DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedUtc DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id)
);

-- Welder Qualifications
CREATE TABLE WelderQualifications (
    QualificationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WelderId UNIQUEIDENTIFIER NOT NULL,
    WelderIdentification NVARCHAR(50) NOT NULL, -- Badge/ID number
    WelderName NVARCHAR(200),
    
    -- Qualification Details
    QualificationStandard NVARCHAR(100), -- "EN ISO 9606-1:2017"
    WeldingProcess NVARCHAR(100),
    MaterialGroup NVARCHAR(50),
    ThicknessRange NVARCHAR(50),
    PipeOuterDiameter NVARCHAR(50), -- For pipe welding
    WeldingPositions NVARCHAR(100),
    
    -- Test Details
    TestDate DATETIME2,
    TestLocation NVARCHAR(200),
    TestingOrganization NVARCHAR(200),
    TestPiece NVARCHAR(100),
    WeldingConsumable NVARCHAR(100),
    
    -- Certificate Details
    CertificateNumber NVARCHAR(100),
    QualificationDate DATETIME2,
    ExpiryDate DATETIME2,
    
    -- Status
    IsValid BIT DEFAULT 1,
    Notes NVARCHAR(MAX),
    
    -- Files
    CertificateDocument VARBINARY(MAX),
    CertificateFileName NVARCHAR(255),
    
    CreatedUtc DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedUtc DATETIME2 DEFAULT GETUTCDATE()
);

-- Inspection Activities
CREATE TABLE InspectionActivities (
    InspectionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProjectId UNIQUEIDENTIFIER NOT NULL,
    ComponentReference NVARCHAR(100),
    InspectionType NVARCHAR(50), -- Visual, MT, UT, RT, PT
    
    -- Planning
    RequiredByExecutionClass BIT DEFAULT 0,
    RequiredBySpecification BIT DEFAULT 0,
    PlannedDate DATETIME2,
    InspectorRequired NVARCHAR(200),
    
    -- Execution
    ActualDate DATETIME2,
    InspectorName NVARCHAR(200),
    InspectorQualification NVARCHAR(100),
    
    -- Results
    InspectionStatus NVARCHAR(20), -- Planned, InProgress, Passed, Failed, Rework
    InspectionStandard NVARCHAR(100), -- EN ISO 5817, EN ISO 17638, etc.
    QualityLevel NVARCHAR(10), -- B, C, D (for EN ISO 5817)
    
    -- Documentation
    InspectionReport VARBINARY(MAX),
    ReportFileName NVARCHAR(255),
    Photos NVARCHAR(MAX), -- JSON array of photo references
    
    -- Non-conformances
    NonConformanceFound BIT DEFAULT 0,
    NonConformanceDescription NVARCHAR(MAX),
    CorrectiveAction NVARCHAR(MAX),
    ReinspectionRequired BIT DEFAULT 0,
    
    CreatedUtc DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedUtc DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId)
);

-- Compliance Requirements Matrix
CREATE TABLE ComplianceRequirements (
    RequirementId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProjectId UNIQUEIDENTIFIER NOT NULL,
    ExecutionClass NVARCHAR(10) NOT NULL,
    
    -- Requirement Details
    RequirementType NVARCHAR(50), -- Material, Welding, Inspection, Documentation
    RequirementCode NVARCHAR(50), -- Unique identifier
    RequirementDescription NVARCHAR(500),
    ApplicableStandard NVARCHAR(100),
    
    -- Status
    ComplianceStatus NVARCHAR(20), -- NotStarted, InProgress, Completed, NonCompliant
    CompletionDate DATETIME2,
    CompletedByUserId NVARCHAR(450),
    
    -- Evidence
    EvidenceDescription NVARCHAR(MAX),
    EvidenceDocuments NVARCHAR(MAX), -- JSON array of document references
    
    -- Due Date
    DueDate DATETIME2,
    Priority NVARCHAR(20), -- High, Medium, Low
    
    CreatedUtc DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedUtc DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId),
    FOREIGN KEY (CompletedByUserId) REFERENCES AspNetUsers(Id)
);
```

### Index Requirements for Performance

```sql
-- Execution Classes
CREATE INDEX IX_ExecutionClasses_ProjectId ON ExecutionClasses(ProjectId);
CREATE INDEX IX_ExecutionClasses_ClassLevel ON ExecutionClasses(ClassLevel);

-- Material Certificates
CREATE INDEX IX_MaterialCertificates_ProfileInventoryId ON MaterialCertificates(ProfileInventoryId);
CREATE INDEX IX_MaterialCertificates_HeatNumber ON MaterialCertificates(HeatNumber);
CREATE INDEX IX_MaterialCertificates_CertificateType ON MaterialCertificates(CertificateType);

-- Welding Procedures
CREATE INDEX IX_WeldingProcedures_WPSNumber ON WeldingProcedures(WPSNumber);
CREATE INDEX IX_WeldingProcedures_BaseMetalGroup ON WeldingProcedures(BaseMetalGroup);
CREATE INDEX IX_WeldingProcedures_WeldingProcess ON WeldingProcedures(WeldingProcess);

-- Welder Qualifications
CREATE INDEX IX_WelderQualifications_WelderId ON WelderQualifications(WelderId);
CREATE INDEX IX_WelderQualifications_WelderIdentification ON WelderQualifications(WelderIdentification);
CREATE INDEX IX_WelderQualifications_ExpiryDate ON WelderQualifications(ExpiryDate);

-- Inspection Activities
CREATE INDEX IX_InspectionActivities_ProjectId ON InspectionActivities(ProjectId);
CREATE INDEX IX_InspectionActivities_InspectionType ON InspectionActivities(InspectionType);
CREATE INDEX IX_InspectionActivities_InspectionStatus ON InspectionActivities(InspectionStatus);
CREATE INDEX IX_InspectionActivities_PlannedDate ON InspectionActivities(PlannedDate);

-- Compliance Requirements
CREATE INDEX IX_ComplianceRequirements_ProjectId ON ComplianceRequirements(ProjectId);
CREATE INDEX IX_ComplianceRequirements_ExecutionClass ON ComplianceRequirements(ExecutionClass);
CREATE INDEX IX_ComplianceRequirements_ComplianceStatus ON ComplianceRequirements(ComplianceStatus);
CREATE INDEX IX_ComplianceRequirements_DueDate ON ComplianceRequirements(DueDate);
```

---

## API Specifications

### Core API Endpoints for EN 1090 Compliance

#### 1. Execution Class Management

```csharp
// POST /api/projects/{projectId}/execution-class
[HttpPost("{projectId}/execution-class")]
[RequireFeature(FeatureKeys.EN1090Compliance)]
public async Task<IActionResult> DetermineExecutionClass(
    Guid projectId, 
    [FromBody] ExecutionClassParameters parameters)
{
    var executionClass = await _en1090Service.DetermineExecutionClassAsync(
        projectId, parameters);
    return Ok(executionClass);
}

// GET /api/projects/{projectId}/execution-class
[HttpGet("{projectId}/execution-class")]
[RequireFeature(FeatureKeys.EN1090Compliance)]
public async Task<IActionResult> GetExecutionClass(Guid projectId)
{
    var executionClass = await _en1090Service.GetExecutionClassAsync(projectId);
    return Ok(executionClass);
}

// PUT /api/projects/{projectId}/execution-class
[HttpPut("{projectId}/execution-class")]
[RequireFeature(FeatureKeys.EN1090Compliance)]
public async Task<IActionResult> UpdateExecutionClass(
    Guid projectId, 
    [FromBody] UpdateExecutionClassRequest request)
{
    await _en1090Service.UpdateExecutionClassAsync(projectId, request);
    return NoContent();
}
```

#### 2. Material Certificate Management

```csharp
// POST /api/inventory/{inventoryId}/certificates
[HttpPost("{inventoryId}/certificates")]
[RequireFeature(FeatureKeys.MaterialTraceability)]
public async Task<IActionResult> UploadCertificate(
    Guid inventoryId, 
    [FromForm] CertificateUploadRequest request)
{
    var certificate = await _certificateService.UploadCertificateAsync(
        inventoryId, request.File, request.Metadata);
    return CreatedAtAction(nameof(GetCertificate), 
        new { certificateId = certificate.CertificateId }, certificate);
}

// GET /api/certificates/{certificateId}
[HttpGet("{certificateId}")]
[RequireFeature(FeatureKeys.MaterialTraceability)]
public async Task<IActionResult> GetCertificate(Guid certificateId)
{
    var certificate = await _certificateService.GetCertificateAsync(certificateId);
    return Ok(certificate);
}

// POST /api/certificates/{certificateId}/verify
[HttpPost("{certificateId}/verify")]
[RequireFeature(FeatureKeys.MaterialTraceability)]
public async Task<IActionResult> VerifyCertificate(
    Guid certificateId, 
    [FromBody] CertificateVerificationRequest request)
{
    await _certificateService.VerifyCertificateAsync(certificateId, request);
    return NoContent();
}
```

#### 3. Welding Management

```csharp
// GET /api/welding/procedures
[HttpGet("procedures")]
[RequireFeature(FeatureKeys.WeldingManagement)]
public async Task<IActionResult> GetWeldingProcedures(
    [FromQuery] WeldingProcedureQuery query)
{
    var procedures = await _weldingService.GetProceduresAsync(query);
    return Ok(procedures);
}

// POST /api/welding/procedures
[HttpPost("procedures")]
[RequireFeature(FeatureKeys.WeldingManagement)]
public async Task<IActionResult> CreateWeldingProcedure(
    [FromBody] CreateWeldingProcedureRequest request)
{
    var procedure = await _weldingService.CreateProcedureAsync(request);
    return CreatedAtAction(nameof(GetWeldingProcedure), 
        new { wpsId = procedure.WPSId }, procedure);
}

// GET /api/welding/qualifications
[HttpGet("qualifications")]
[RequireFeature(FeatureKeys.WeldingManagement)]
public async Task<IActionResult> GetWelderQualifications(
    [FromQuery] WelderQualificationQuery query)
{
    var qualifications = await _weldingService.GetQualificationsAsync(query);
    return Ok(qualifications);
}
```

#### 4. Inspection Management

```csharp
// POST /api/projects/{projectId}/inspection-plan
[HttpPost("{projectId}/inspection-plan")]
[RequireFeature(FeatureKeys.InspectionManagement)]
public async Task<IActionResult> GenerateInspectionPlan(Guid projectId)
{
    var inspectionPlan = await _inspectionService.GenerateInspectionPlanAsync(projectId);
    return Ok(inspectionPlan);
}

// GET /api/projects/{projectId}/inspections
[HttpGet("{projectId}/inspections")]
[RequireFeature(FeatureKeys.InspectionManagement)]
public async Task<IActionResult> GetInspections(
    Guid projectId, 
    [FromQuery] InspectionQuery query)
{
    var inspections = await _inspectionService.GetInspectionsAsync(projectId, query);
    return Ok(inspections);
}

// POST /api/inspections/{inspectionId}/complete
[HttpPost("{inspectionId}/complete")]
[RequireFeature(FeatureKeys.InspectionManagement)]
public async Task<IActionResult> CompleteInspection(
    Guid inspectionId, 
    [FromBody] CompleteInspectionRequest request)
{
    await _inspectionService.CompleteInspectionAsync(inspectionId, request);
    return NoContent();
}
```

#### 5. Compliance Reporting

```csharp
// GET /api/projects/{projectId}/compliance-status
[HttpGet("{projectId}/compliance-status")]
[RequireFeature(FeatureKeys.EN1090Compliance)]
public async Task<IActionResult> GetComplianceStatus(Guid projectId)
{
    var status = await _complianceService.GetComplianceStatusAsync(projectId);
    return Ok(status);
}

// POST /api/projects/{projectId}/declaration-of-performance
[HttpPost("{projectId}/declaration-of-performance")]
[RequireFeature(FeatureKeys.EN1090Compliance)]
public async Task<IActionResult> GenerateDeclarationOfPerformance(Guid projectId)
{
    var document = await _complianceService.GenerateDeclarationOfPerformanceAsync(projectId);
    return File(document.Content, document.MimeType, document.FileName);
}
```

### Data Transfer Objects (DTOs)

```csharp
public class ExecutionClassParameters
{
    public ConsequenceClass ConsequenceClass { get; set; }
    public string SeismicCategory { get; set; }
    public string ServiceCategory { get; set; }
    public int DesignWorkingLife { get; set; }
    public string EnvironmentalExposure { get; set; }
    public string BuildingType { get; set; }
    public string Justification { get; set; }
}

public class ExecutionClassResult
{
    public ExecutionClass ClassLevel { get; set; }
    public string Justification { get; set; }
    public List<ComplianceRequirement> Requirements { get; set; }
    public DateTime DeterminationDate { get; set; }
}

public class MaterialCertificateDto
{
    public Guid CertificateId { get; set; }
    public string CertificateType { get; set; }
    public string CertificateNumber { get; set; }
    public string HeatNumber { get; set; }
    public string ManufacturerName { get; set; }
    public string SteelGrade { get; set; }
    public Dictionary<string, object> ChemicalComposition { get; set; }
    public Dictionary<string, object> MechanicalProperties { get; set; }
    public DateTime CertificateDate { get; set; }
    public bool IsVerified { get; set; }
}

public class WeldingProcedureDto
{
    public Guid WPSId { get; set; }
    public string WPSNumber { get; set; }
    public string Title { get; set; }
    public string BaseMetalGroup { get; set; }
    public string WeldingProcess { get; set; }
    public string FillerMetalType { get; set; }
    public string ThicknessRange { get; set; }
    public string WeldingPosition { get; set; }
    public DateTime QualificationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
}

public class InspectionActivityDto
{
    public Guid InspectionId { get; set; }
    public string ComponentReference { get; set; }
    public string InspectionType { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string InspectionStatus { get; set; }
    public string InspectorName { get; set; }
    public string QualityLevel { get; set; }
    public bool NonConformanceFound { get; set; }
    public string NonConformanceDescription { get; set; }
}
```

---

## User Interface Requirements

### Navigation Structure Enhancement

Add to main navigation:
```
🏗️ Steel Construction
├── 📋 Project Configuration
│   ├── Execution Class Setup
│   ├── Compliance Requirements
│   └── Standards Configuration
├── 🔍 Material Compliance
│   ├── Certificate Management
│   ├── Traceability Reports
│   └── Material Verification
├── ⚡ Welding Management
│   ├── WPS/WPQR Library
│   ├── Welder Qualifications
│   └── Welding Coordination
├── 🧪 Quality & Inspection
│   ├── Inspection Planning
│   ├── NDT Management
│   └── Quality Reports
└── 📊 Compliance Dashboard
    ├── Project Status
    ├── Outstanding Items
    └── Audit Reports
```

### Key UI Components

#### 1. Execution Class Wizard
**Component Path:** `/Components/EN1090/ExecutionClassWizard.razor`

```razor
@using Manimp.Services.EN1090
@inject IEN1090Service EN1090Service

<MudStepper @ref="stepper" Linear="true">
    <MudStep Title="Project Information">
        <MudGrid>
            <MudItem xs="12" md="6">
                <MudSelect @bind-Value="parameters.ConsequenceClass" 
                           Label="Consequence Class">
                    <MudSelectItem Value="ConsequenceClass.CC1">CC1 - Low Risk</MudSelectItem>
                    <MudSelectItem Value="ConsequenceClass.CC2">CC2 - Medium Risk</MudSelectItem>
                    <MudSelectItem Value="ConsequenceClass.CC3">CC3 - High Risk</MudSelectItem>
                </MudSelect>
            </MudItem>
            <MudItem xs="12" md="6">
                <MudSelect @bind-Value="parameters.SeismicCategory" 
                           Label="Seismic Category">
                    <MudSelectItem Value="Low">Low</MudSelectItem>
                    <MudSelectItem Value="Medium">Medium</MudSelectItem>
                    <MudSelectItem Value="High">High</MudSelectItem>
                    <MudSelectItem Value="VeryHigh">Very High</MudSelectItem>
                </MudSelect>
            </MudItem>
        </MudGrid>
    </MudStep>
    
    <MudStep Title="Design Parameters">
        <MudGrid>
            <MudItem xs="12" md="6">
                <MudNumericField @bind-Value="parameters.DesignWorkingLife" 
                                 Label="Design Working Life (years)" 
                                 Min="0" Max="200" />
            </MudItem>
            <MudItem xs="12" md="6">
                <MudSelect @bind-Value="parameters.EnvironmentalExposure" 
                           Label="Environmental Exposure">
                    <MudSelectItem Value="C1">C1 - Dry Indoor</MudSelectItem>
                    <MudSelectItem Value="C2">C2 - Wet Indoor</MudSelectItem>
                    <MudSelectItem Value="C3">C3 - Outdoor Sheltered</MudSelectItem>
                    <MudSelectItem Value="C4">C4 - Outdoor Exposed</MudSelectItem>
                    <MudSelectItem Value="C5">C5 - Marine</MudSelectItem>
                </MudSelect>
            </MudItem>
        </MudGrid>
    </MudStep>
    
    <MudStep Title="Review & Determination">
        @if (executionClassResult != null)
        {
            <MudAlert Severity="Severity.Success">
                <strong>Determined Execution Class: @executionClassResult.ClassLevel</strong>
            </MudAlert>
            <MudText Typo="Typo.body1">@executionClassResult.Justification</MudText>
            
            <MudDataGrid Items="@executionClassResult.Requirements" 
                         Class="mt-4">
                <Columns>
                    <PropertyColumn Property="x => x.RequirementType" Title="Type" />
                    <PropertyColumn Property="x => x.RequirementDescription" Title="Requirement" />
                    <PropertyColumn Property="x => x.ApplicableStandard" Title="Standard" />
                </Columns>
            </MudDataGrid>
        }
    </MudStep>
</MudStepper>
```

#### 2. Compliance Dashboard
**Component Path:** `/Components/EN1090/ComplianceDashboard.razor`

```razor
<MudGrid>
    <MudItem xs="12" md="3">
        <MudCard>
            <MudCardContent>
                <div class="d-flex align-center">
                    <MudIcon Icon="@Icons.Material.Filled.Assignment" 
                             Color="Color.Primary" Size="Size.Large" />
                    <div class="ml-3">
                        <MudText Typo="Typo.h6">@completedRequirements</MudText>
                        <MudText Typo="Typo.caption">Completed Requirements</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>
    
    <MudItem xs="12" md="3">
        <MudCard>
            <MudCardContent>
                <div class="d-flex align-center">
                    <MudIcon Icon="@Icons.Material.Filled.Schedule" 
                             Color="Color.Warning" Size="Size.Large" />
                    <div class="ml-3">
                        <MudText Typo="Typo.h6">@pendingRequirements</MudText>
                        <MudText Typo="Typo.caption">Pending Requirements</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>
    
    <MudItem xs="12" md="3">
        <MudCard>
            <MudCardContent>
                <div class="d-flex align-center">
                    <MudIcon Icon="@Icons.Material.Filled.Warning" 
                             Color="Color.Error" Size="Size.Large" />
                    <div class="ml-3">
                        <MudText Typo="Typo.h6">@overdueRequirements</MudText>
                        <MudText Typo="Typo.caption">Overdue Requirements</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>
    
    <MudItem xs="12" md="3">
        <MudCard>
            <MudCardContent>
                <div class="d-flex align-center">
                    <MudIcon Icon="@Icons.Material.Filled.CheckCircle" 
                             Color="Color.Success" Size="Size.Large" />
                    <div class="ml-3">
                        <MudText Typo="Typo.h6">@Math.Round(compliancePercentage, 1)%</MudText>
                        <MudText Typo="Typo.caption">Compliance Level</MudText>
                    </div>
                </div>
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>

<MudGrid Class="mt-4">
    <MudItem xs="12" md="8">
        <MudCard>
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.h6">Outstanding Requirements</MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                <MudDataGrid Items="@outstandingRequirements" 
                             Dense="true" Hover="true">
                    <Columns>
                        <PropertyColumn Property="x => x.RequirementDescription" Title="Requirement" />
                        <PropertyColumn Property="x => x.DueDate" Title="Due Date" Format="MM/dd/yyyy" />
                        <PropertyColumn Property="x => x.Priority" Title="Priority">
                            <CellTemplate>
                                <MudChip Color="@GetPriorityColor(context.Item.Priority)" Size="Size.Small">
                                    @context.Item.Priority
                                </MudChip>
                            </CellTemplate>
                        </PropertyColumn>
                    </Columns>
                </MudDataGrid>
            </MudCardContent>
        </MudCard>
    </MudItem>
    
    <MudItem xs="12" md="4">
        <MudCard>
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.h6">Upcoming Inspections</MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                <MudTimeline>
                    @foreach (var inspection in upcomingInspections)
                    {
                        <MudTimelineItem Color="Color.Primary" Variant="Variant.Filled" Size="Size.Small">
                            <ItemContent>
                                <div class="d-flex">
                                    <MudText Typo="Typo.body2" class="flex-grow-1">
                                        @inspection.ComponentReference
                                    </MudText>
                                    <MudText Typo="Typo.caption">
                                        @inspection.PlannedDate?.ToString("MM/dd")
                                    </MudText>
                                </div>
                                <MudText Typo="Typo.caption" Class="text-muted">
                                    @inspection.InspectionType
                                </MudText>
                            </ItemContent>
                        </MudTimelineItem>
                    }
                </MudTimeline>
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>
```

### Responsive Design Considerations

- **Mobile-first approach** for inspection forms and material verification
- **Tablet optimization** for welding procedure selection and review
- **Desktop focus** for complex compliance dashboards and reporting
- **Offline capability** for inspection activities in production environments

---

## Workflow Integration

### Integration with Existing Manimp Features

#### 1. Project Management Integration

**Enhanced Project Creation Workflow:**
```
Project Creation
├── Basic Project Info (existing)
├── Execution Class Determination (new)
├── Compliance Requirements Generation (new)
├── Material Specifications (enhanced)
└── Resource Planning (enhanced with compliance activities)
```

**Implementation Points:**
- Add execution class determination step to project wizard
- Auto-generate compliance requirements based on execution class
- Integrate inspection planning with project scheduling
- Link material specifications to certificate requirements

#### 2. Inventory Management Enhancement

**Material Receipt Process:**
```
Material Receipt
├── Existing Inventory Entry
├── Certificate Upload (new)
├── Certificate Verification (new)
├── Traceability Linking (new)
└── Compliance Status Update (new)
```

**New Features:**
- Mandatory certificate upload for certain execution classes
- Heat/batch number validation against inventory items
- Automatic compliance checking against material specifications
- Integration with material usage tracking for traceability

#### 3. Procurement Integration

**Enhanced Purchase Order Process:**
```
Purchase Order Creation
├── Material Specification (existing)
├── Certificate Requirements (new)
├── Inspection Requirements (new)
├── Supplier Qualification Check (new)
└── Compliance Documentation (new)
```

**Value Additions:**
- Automatic certificate requirement specification based on execution class
- Supplier qualification tracking for quality assurance
- Integration with incoming inspection planning
- Compliance cost tracking and budgeting

### Feature Gating Integration

**Feature Dependencies:**
```csharp
public static class EN1090FeatureKeys
{
    public const string BasicCompliance = "en1090-basic-compliance";
    public const string MaterialTraceability = "en1090-material-traceability";
    public const string WeldingManagement = "en1090-welding-management";
    public const string InspectionManagement = "en1090-inspection-management";
    public const string AdvancedReporting = "en1090-advanced-reporting";
}

public static class FeatureTiers
{
    public static readonly Dictionary<string, int> EN1090Features = new()
    {
        [EN1090FeatureKeys.BasicCompliance] = 2, // Professional Plan
        [EN1090FeatureKeys.MaterialTraceability] = 2, // Professional Plan
        [EN1090FeatureKeys.WeldingManagement] = 3, // Enterprise Plan
        [EN1090FeatureKeys.InspectionManagement] = 3, // Enterprise Plan
        [EN1090FeatureKeys.AdvancedReporting] = 3 // Enterprise Plan
    };
}
```

---

## Testing and Validation

### Unit Testing Requirements

#### 1. Execution Class Determination Engine Tests

```csharp
[TestClass]
public class ExecutionClassDeterminationTests
{
    private IEN1090Service _en1090Service;
    
    [TestInitialize]
    public void Setup()
    {
        _en1090Service = new EN1090Service();
    }
    
    [TestMethod]
    public void DetermineExecutionClass_LowRiskParameters_ReturnsEXC1()
    {
        // Arrange
        var parameters = new ExecutionClassParameters
        {
            ConsequenceClass = ConsequenceClass.CC1,
            SeismicCategory = "Low",
            ServiceCategory = "S0",
            DesignWorkingLife = 50,
            EnvironmentalExposure = "C1"
        };
        
        // Act
        var result = _en1090Service.DetermineExecutionClass(parameters);
        
        // Assert
        Assert.AreEqual(ExecutionClass.EXC1, result.ClassLevel);
    }
    
    [TestMethod]
    public void DetermineExecutionClass_HighRiskParameters_ReturnsEXC3OrHigher()
    {
        // Arrange
        var parameters = new ExecutionClassParameters
        {
            ConsequenceClass = ConsequenceClass.CC3,
            SeismicCategory = "High",
            ServiceCategory = "S1",
            DesignWorkingLife = 100,
            EnvironmentalExposure = "C5"
        };
        
        // Act
        var result = _en1090Service.DetermineExecutionClass(parameters);
        
        // Assert
        Assert.IsTrue(result.ClassLevel >= ExecutionClass.EXC3);
    }
}
```

#### 2. Material Certificate Validation Tests

```csharp
[TestClass]
public class MaterialCertificateTests
{
    [TestMethod]
    public void ValidateCertificate_ValidEN10204Type31_ReturnsTrue()
    {
        // Arrange
        var certificate = new MaterialCertificate
        {
            CertificateType = "EN10204_3_1",
            HeatNumber = "12345ABC",
            ChemicalComposition = GetValidChemicalComposition(),
            MechanicalProperties = GetValidMechanicalProperties()
        };
        
        // Act
        var result = _certificateService.ValidateCertificate(certificate);
        
        // Assert
        Assert.IsTrue(result.IsValid);
    }
    
    [TestMethod]
    public void ValidateCertificate_MissingHeatNumber_ReturnsFalse()
    {
        // Arrange
        var certificate = new MaterialCertificate
        {
            CertificateType = "EN10204_3_1",
            HeatNumber = null // Missing heat number
        };
        
        // Act
        var result = _certificateService.ValidateCertificate(certificate);
        
        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Heat number")));
    }
}
```

### Integration Testing

#### 1. End-to-End Compliance Workflow Test

```csharp
[TestClass]
public class ComplianceWorkflowIntegrationTests
{
    [TestMethod]
    public async Task CompleteComplianceWorkflow_NewProject_GeneratesAllRequiredDocuments()
    {
        // Arrange
        var project = await CreateTestProject();
        var parameters = GetStandardExecutionClassParameters();
        
        // Act - Determine execution class
        var executionClass = await _en1090Service.DetermineExecutionClassAsync(
            project.ProjectId, parameters);
        
        // Upload material certificates
        await UploadTestMaterialCertificates(project.ProjectId);
        
        // Complete inspection activities
        await CompleteTestInspections(project.ProjectId);
        
        // Generate compliance documents
        var documents = await _complianceService.GenerateComplianceDocumentsAsync(
            project.ProjectId);
        
        // Assert
        Assert.IsNotNull(executionClass);
        Assert.IsTrue(documents.ContainsKey("DeclarationOfPerformance"));
        Assert.IsTrue(documents.ContainsKey("MaterialTraceabilityReport"));
    }
}
```

### User Acceptance Testing Scenarios

#### Scenario 1: Project Manager Determines Execution Class
1. **Given:** A new steel construction project
2. **When:** Project manager enters project parameters
3. **Then:** System automatically determines appropriate execution class
4. **And:** Generates list of compliance requirements
5. **And:** Creates inspection schedule

#### Scenario 2: Quality Manager Uploads Material Certificates
1. **Given:** Materials received with certificates
2. **When:** Quality manager uploads certificate documents
3. **Then:** System validates certificate information
4. **And:** Links certificates to inventory items
5. **And:** Updates material traceability records

#### Scenario 3: Inspector Completes NDT Activities
1. **Given:** Scheduled NDT inspection
2. **When:** Inspector records inspection results
3. **Then:** System updates compliance status
4. **And:** Triggers next workflow steps if required
5. **And:** Generates inspection reports

---

## Deployment Considerations

### Environment Configuration

#### Development Environment
```json
{
  "EN1090": {
    "EnableComplianceFeatures": true,
    "StrictValidation": false,
    "TestMode": true,
    "AutoGenerateTestData": true
  },
  "FeatureGating": {
    "EN1090Features": {
      "BasicCompliance": true,
      "MaterialTraceability": true,
      "WeldingManagement": true,
      "InspectionManagement": true,
      "AdvancedReporting": true
    }
  }
}
```

#### Production Environment
```json
{
  "EN1090": {
    "EnableComplianceFeatures": true,
    "StrictValidation": true,
    "TestMode": false,
    "RequireDigitalSignatures": true,
    "AuditLogging": true
  }
}
```

### Database Migration Strategy

#### Phase 1: Core Tables
```bash
# Add execution class and basic compliance tables
dotnet ef migrations add EN1090_Phase1_BasicCompliance

# Add material certificate tables
dotnet ef migrations add EN1090_Phase1_MaterialCertificates
```

#### Phase 2: Welding and Inspection
```bash
# Add welding management tables
dotnet ef migrations add EN1090_Phase2_WeldingManagement

# Add inspection and NDT tables
dotnet ef migrations add EN1090_Phase2_InspectionManagement
```

#### Phase 3: Advanced Features
```bash
# Add document generation and advanced reporting
dotnet ef migrations add EN1090_Phase3_AdvancedReporting
```

### Performance Considerations

#### Database Optimization
- **Indexes:** Add appropriate indexes for compliance queries
- **Partitioning:** Consider partitioning large inspection tables by date
- **Archiving:** Implement data archiving for old compliance records

#### File Storage
- **Certificates:** Store in Azure Blob Storage with CDN
- **Reports:** Generate on-demand to reduce storage costs
- **Backups:** Ensure compliance with data retention requirements

#### Caching Strategy
- **Execution Class Rules:** Cache determination logic
- **Certificate Validation:** Cache validation results
- **Compliance Status:** Cache project compliance status

### Security Requirements

#### Data Protection
- **Encryption at rest** for all compliance documents
- **Encryption in transit** for all API communications
- **Digital signatures** for critical compliance documents
- **Audit logging** for all compliance-related activities

#### Access Control
- **Role-based permissions** for different compliance activities
- **Document-level security** for sensitive certificates
- **API rate limiting** to prevent abuse
- **Multi-factor authentication** for compliance admin functions

#### Compliance with Regulations
- **GDPR compliance** for EU operations
- **Data retention policies** according to EN 1090 requirements
- **Right to erasure** considerations for project data
- **Cross-border data transfer** compliance

---

## Summary and Next Steps

### Immediate Actions (Next 30 days)
1. **Create database migration** for execution class tables
2. **Implement execution class determination engine**
3. **Build basic compliance dashboard**
4. **Add material certificate upload functionality**

### Short-term Goals (3 months)
1. **Complete material traceability system**
2. **Implement welding procedure management**
3. **Create inspection planning module**
4. **Develop basic compliance reporting**

### Long-term Objectives (6-12 months)
1. **Full document generation automation**
2. **Advanced analytics and trending**
3. **Mobile application for inspectors**
4. **Integration with external systems**

### Success Metrics
- **Time to compliance:** Reduce project compliance time by 50%
- **Error reduction:** Decrease compliance errors by 75%
- **User adoption:** Achieve 90% user adoption within 6 months
- **Customer satisfaction:** Maintain 4.5+ rating for compliance features

This implementation guide provides the technical foundation for integrating comprehensive EN 1090 compliance into the Manimp application, ensuring both regulatory adherence and operational efficiency.