# EN 1090 Steel Construction Manufacturing Compliance Guide

This document provides comprehensive guidance on EN 1090 requirements for steel construction manufacturing, with specific focus on aspects necessary for software development and compliance tracking within the Manimp application.

## Table of Contents

1. [Overview of EN 1090](#overview-of-en-1090)
2. [CE Marking Scope](#ce-marking-scope)
3. [Execution Classes (EXC1-EXC4)](#execution-classes-exc1-exc4)
4. [Factory Production Control (FPC) and AVCP](#factory-production-control-fpc-and-avcp)
5. [Welding Requirements](#welding-requirements)
6. [Material Traceability and Inspection](#material-traceability-and-inspection)
7. [Non-Destructive Testing (NDT)](#non-destructive-testing-ndt)
8. [Documentation Package Requirements](#documentation-package-requirements)
9. [Software Implementation Recommendations](#software-implementation-recommendations)
10. [References and Standards](#references-and-standards)

---

## Overview of EN 1090

### EN 1090-1: General Requirements and Rules for Conformity Assessment
**Scope:** Technical requirements for CE marking of structural steel and aluminum products.

**Key Components:**
- Factory Production Control (FPC) requirements
- Assessment and Verification of Constancy of Performance (AVCP) procedures
- Essential characteristics and performance criteria
- Conformity assessment procedures
- CE marking obligations

### EN 1090-2: Technical Requirements for Steel Structures
**Scope:** Technical requirements for the execution of steel structures.

**Key Components:**
- Material specifications and requirements
- Welding procedures and qualification requirements
- Non-destructive testing specifications
- Assembly and erection requirements
- Dimensional tolerances
- Surface preparation and protection

### EN 1090-3: Technical Requirements for Aluminum Structures
**Scope:** Technical requirements for the execution of aluminum structures (not covered in detail in this document as focus is on steel).

---

## CE Marking Scope

### When CE Marking is Required

**Structural Steel Products requiring CE marking:**
- Load-bearing structural steel components and kits
- Structural steel elements intended for incorporation into buildings and civil engineering works
- Products covered by the Construction Products Regulation (CPR)

**Products NOT requiring CE marking:**
- Non-structural steel components
- Maintenance and repair work on existing structures
- One-off structures designed by and under responsibility of a civil engineer

### Essential Characteristics for CE Marking

1. **Reaction to Fire**
   - Classification according to EN 13501-1
   - Typically Class A1 for structural steel

2. **Release of Corrosive Substances**
   - NPD (No Performance Determined) for most structural applications

3. **Resistance to Fatigue**
   - Where relevant, according to EN 1993-1-9

4. **Stiffness**
   - Elastic modulus values
   - Shear modulus values

5. **Resistance**
   - Yield strength
   - Ultimate tensile strength
   - Fracture toughness (where relevant)

6. **Durability**
   - Resistance to corrosion
   - Surface protection systems

---

## Execution Classes (EXC1-EXC4)

### Definition and Application

Execution classes define the level of requirements for manufacturing tolerances, welding quality, and inspection procedures.

### EXC1 - Lowest Execution Class
**Typical Applications:**
- Non-critical structural elements
- Secondary structural members
- Low seismic areas
- Simple agricultural buildings

**Requirements:**
- Basic welding procedures
- Visual inspection only
- Standard material certificates
- Basic dimensional tolerances

### EXC2 - Standard Execution Class
**Typical Applications:**
- Standard building construction
- Ordinary industrial structures
- Most commercial buildings
- Standard infrastructure projects

**Requirements:**
- Qualified welding procedures (WPS/WPQR)
- Visual inspection + limited NDT
- Material certificates with enhanced properties
- Enhanced dimensional tolerances

### EXC3 - High Execution Class
**Typical Applications:**
- Important public buildings
- High-rise structures
- Bridges and infrastructure
- Seismic design applications
- Offshore structures

**Requirements:**
- Fully qualified welding procedures
- Comprehensive NDT requirements
- Full material traceability
- Strict dimensional tolerances
- Enhanced FPC requirements

### EXC4 - Highest Execution Class
**Typical Applications:**
- Critical infrastructure
- Nuclear facilities
- Major bridges
- Exceptional seismic conditions
- Structures with extreme consequences of failure

**Requirements:**
- Most stringent welding procedures
- Extensive NDT and inspection
- Complete material traceability
- Tightest dimensional tolerances
- Maximum FPC requirements

### Determination Criteria

**Factors influencing execution class selection:**
1. **Consequence Class (CC)** - Impact of failure
2. **Seismic Design Category**
3. **Service Category** - Fatigue considerations
4. **Inspection Possibility** during service life
5. **Design Working Life**
6. **Environmental Conditions**

**Software Integration:** The application should allow project managers to:
- Select appropriate execution class based on project parameters
- Apply corresponding inspection and documentation requirements
- Track compliance with class-specific procedures

---

## Factory Production Control (FPC) and AVCP

### Factory Production Control (FPC) System

**Purpose:** Ensure that manufactured products consistently meet declared performance characteristics.

#### FPC Requirements by Product Category

**System 2+ (Most structural steel products):**
- Initial Type Testing (ITT) by notified body
- Factory Production Control by manufacturer
- Surveillance of FPC by notified body
- Declaration of Performance (DoP) by manufacturer

#### Key FPC Elements

1. **Management System**
   - Quality policy and objectives
   - Organizational structure and responsibilities
   - Document control procedures
   - Management review processes

2. **Control of Raw Materials**
   - Incoming inspection procedures
   - Material identification and traceability
   - Storage and handling requirements
   - Non-conforming material control

3. **Production Control**
   - Process control procedures
   - Work instruction management
   - Equipment calibration and maintenance
   - Environmental control

4. **Product Testing and Inspection**
   - Inspection and test plans
   - Measuring equipment control
   - Non-conforming product control
   - Corrective and preventive actions

5. **Record Keeping**
   - Production records
   - Test results documentation
   - Non-conformance records
   - Training records

### Assessment and Verification of Constancy of Performance (AVCP)

**AVCP Tasks:**
- Initial Type Testing (ITT)
- Surveillance, assessment and evaluation of FPC
- Audit testing of samples taken at factory
- Regular surveillance visits

**Notified Body Responsibilities:**
- Assess and approve FPC system
- Conduct regular surveillance
- Issue certificates of constancy of performance
- Maintain oversight of production quality

**Software Integration Requirements:**
- Track FPC documentation and procedures
- Schedule and record surveillance activities
- Maintain certification status and renewal dates
- Generate compliance reports for audits

---

## Welding Requirements

### Welding Procedure Specification (WPS)

**Essential Variables:**
- Base material specification and group
- Welding process(es)
- Filler material type and classification
- Joint design and preparation
- Welding positions
- Pre-heat and interpass temperature
- Post-weld heat treatment
- Gas composition and flow rate

**WPS Documentation Requirements:**
- Material combinations covered
- Thickness ranges applicable
- Essential and non-essential variables
- Welding technique details
- Quality requirements reference

### Welding Procedure Qualification Record (WPQR)

**Test Requirements:**
- Tensile testing
- Bend testing (face, root, side)
- Impact testing (where required)
- Macro-examination
- Hardness testing (where specified)

**Documentation Requirements:**
- Test piece preparation details
- Welding parameters used
- Test results and acceptance criteria
- Qualification date and validity
- Welder/operator identification

### Welder Qualifications

**Qualification Standards:**
- EN ISO 9606-1 (Arc welding of steels)
- EN ISO 14732 (Personnel requirements)

**Qualification Scope:**
- Welding process(es)
- Material group(s)
- Thickness range
- Welding positions
- Weld metal type

**Validity and Renewal:**
- Standard validity: 2 years
- Extension possible with continuous employment
- Re-qualification requirements
- Record keeping obligations

### Welding Coordination - ISO 3834 Alignment

**Coordination Levels:**
- **Level B1:** Comprehensive requirements (EXC3/EXC4)
- **Level B2:** Standard requirements (EXC2/EXC3)
- **Level B3:** Elementary requirements (EXC1/EXC2)

**Coordinator Responsibilities:**
- Review welding requirements
- Establish welding procedures
- Coordinate qualification activities
- Supervise welding activities
- Ensure quality requirements compliance

**Software Integration:**
- Maintain welder qualification database
- Track WPS/WPQR status and validity
- Schedule re-qualification activities
- Link welding procedures to specific projects/components
- Generate welding documentation packages

---

## Material Traceability and Inspection

### Material Traceability Requirements

#### Level 1 Traceability (EXC1/EXC2)
- Material certificates (EN 10204 Type 3.1)
- Basic chemical composition
- Mechanical properties verification
- Heat/batch number tracking

#### Level 2 Traceability (EXC3)
- Enhanced material certificates
- Chemical analysis verification
- Mechanical testing validation
- Detailed heat treatment records
- Dimensional verification

#### Level 3 Traceability (EXC4)
- Complete material pedigree
- Full chemical and mechanical testing
- Heat treatment validation
- Ultrasonic testing of plates
- Complete dimensional verification

### Inspection Requirements

#### Incoming Material Inspection
**Visual Inspection:**
- Surface condition assessment
- Dimensional verification
- Marking and identification check
- Damage assessment

**Documentation Verification:**
- Certificate authenticity
- Specification compliance
- Heat number correlation
- Test result validation

#### In-Process Inspection
**Preparation Stage:**
- Cutting accuracy verification
- Edge preparation quality
- Fit-up dimensional checks
- Cleanliness verification

**Assembly Stage:**
- Joint geometry verification
- Gap and alignment checks
- Temporary attachment inspection
- Access for welding verification

### Software Integration Requirements

**Material Management:**
- Digital certificate storage and verification
- Heat/batch number tracking throughout production
- Automatic compliance checking against specifications
- Integration with procurement and inventory systems

**Inspection Documentation:**
- Digital inspection checklists
- Photo documentation capability
- Non-conformance tracking and resolution
- Inspection history and trending

---

## Non-Destructive Testing (NDT)

### NDT Requirements by Execution Class

#### EXC1
- Visual inspection only
- Basic dimensional checks
- Limited penetrant testing (if specified)

#### EXC2
- Visual inspection (100%)
- Magnetic particle testing (selected welds)
- Ultrasonic testing (critical welds)
- Radiographic testing (if specified)

#### EXC3
- Visual inspection (100%)
- Magnetic particle testing (extensive)
- Ultrasonic testing (comprehensive)
- Radiographic testing (selected areas)
- Penetrant testing (critical areas)

#### EXC4
- Visual inspection (100%)
- Magnetic particle testing (100% of accessible welds)
- Ultrasonic testing (100% of full penetration welds)
- Radiographic testing (comprehensive)
- Penetrant testing (all accessible areas)

### NDT Methods and Applications

#### Visual Testing (VT)
**Applications:**
- All welded connections
- Surface preparation assessment
- Assembly verification
- Final inspection

**Acceptance Criteria:**
- EN ISO 5817 quality levels
- Surface crack detection
- Dimensional compliance
- Profile and finish verification

#### Magnetic Particle Testing (MT)
**Applications:**
- Ferromagnetic materials only
- Surface and near-surface defects
- Weld toe inspection
- Base material examination

**Procedures:**
- EN ISO 17638
- Equipment calibration requirements
- Technique validation
- Acceptance criteria reference

#### Ultrasonic Testing (UT)
**Applications:**
- Full penetration welds
- Thick section materials
- Internal defect detection
- Thickness measurement

**Procedures:**
- EN ISO 17640
- Probe selection and calibration
- Scanning techniques
- Defect evaluation and sizing

#### Radiographic Testing (RT)
**Applications:**
- Full penetration welds
- Critical joint configurations
- Internal defect characterization
- Archive documentation

**Procedures:**
- EN ISO 17636
- Exposure technique optimization
- Image quality verification
- Film/digital image interpretation

### NDT Personnel Qualification

**Qualification Standards:**
- EN ISO 9712 (Certification of NDT personnel)
- Method-specific training requirements
- Practical examination requirements
- Certification maintenance

**Qualification Levels:**
- **Level 1:** Perform specific tests under supervision
- **Level 2:** Perform and interpret tests independently
- **Level 3:** Establish procedures and supervise operations

### Software Integration for NDT

**Planning and Scheduling:**
- Automatic NDT requirement generation based on execution class
- Integration with production schedules
- Resource planning and allocation
- Equipment calibration tracking

**Documentation and Reporting:**
- Digital NDT report generation
- Image/data storage and retrieval
- Non-conformance tracking and resolution
- Statistical analysis and trending

---

## Documentation Package Requirements

### Essential Documentation Components

#### 1. Declaration of Performance (DoP)
**Required Information:**
- Unique identification code
- Product type and intended use
- Manufacturer details
- Authorized representative (if applicable)
- System of AVCP
- Harmonized standard reference
- Notified body details
- Declared performance for essential characteristics
- Performance of product related to durability

#### 2. CE Marking Certificate
**Components:**
- CE marking symbol
- Identification number of notified body
- Last two digits of year of affixing
- Reference to DoP
- Product identification information

#### 3. Manufacturing Documentation
**Production Records:**
- Material traceability records
- Welding procedure records
- Inspection and test reports
- Non-conformance and corrective action records
- Personnel qualification records

#### 4. Quality Documentation
**FPC Records:**
- Quality manual and procedures
- Training records
- Equipment calibration records
- Internal audit reports
- Management review records

### Documentation Management Requirements

#### Storage and Retention
- **Minimum retention period:** 10 years from production date
- **Digital storage:** Recommended for accessibility and preservation
- **Backup procedures:** Essential for business continuity
- **Access control:** Ensure confidentiality and integrity

#### Traceability Linking
- **Product identification:** Unique marking systems
- **Document cross-referencing:** Clear linkage between related documents
- **Version control:** Document revision management
- **Archive management:** Systematic organization and retrieval

### Software Implementation for Documentation

#### Document Management System
**Core Features:**
- Digital document storage and version control
- Automated document generation from production data
- Cross-reference linking between related documents
- Search and retrieval capabilities
- Access control and security

#### Compliance Tracking
**Functionality:**
- Automatic compliance checking against standards
- Document completeness verification
- Expiration date tracking and alerts
- Audit trail maintenance
- Regulatory reporting capabilities

---

## Software Implementation Recommendations

### 1. Project Configuration Module

#### Execution Class Determination
**Input Parameters:**
- Building type and use
- Consequence class (CC1, CC2, CC3)
- Seismic design category
- Service category for fatigue
- Design working life
- Environmental exposure

**Automated Features:**
- Rule-based execution class assignment
- Requirement matrix generation
- Inspection schedule creation
- Documentation checklist generation

#### Integration Points
```csharp
public class ProjectConfiguration
{
    public ExecutionClass DetermineExecutionClass(ProjectParameters parameters)
    {
        // Implementation of EN 1090 decision matrix
        // Returns EXC1, EXC2, EXC3, or EXC4
    }
    
    public RequirementSet GenerateRequirements(ExecutionClass execClass)
    {
        // Generate specific requirements based on execution class
        // Returns welding, inspection, and documentation requirements
    }
}
```

### 2. Material Management Enhancement

#### Traceability System
**Database Schema Extensions:**
```sql
-- Material Certificates Table
CREATE TABLE MaterialCertificates (
    CertificateId UNIQUEIDENTIFIER PRIMARY KEY,
    ProfileInventoryId UNIQUEIDENTIFIER REFERENCES ProfileInventory(Id),
    CertificateType NVARCHAR(50), -- EN 10204 Type 2.1, 3.1, 3.2
    HeatNumber NVARCHAR(100),
    ChemicalComposition NVARCHAR(MAX), -- JSON format
    MechanicalProperties NVARCHAR(MAX), -- JSON format
    CertificateDocument VARBINARY(MAX),
    IssuedDate DATETIME2,
    CertifyingBody NVARCHAR(200)
);

-- Material Test Results Table
CREATE TABLE MaterialTestResults (
    TestResultId UNIQUEIDENTIFIER PRIMARY KEY,
    CertificateId UNIQUEIDENTIFIER REFERENCES MaterialCertificates(CertificateId),
    TestType NVARCHAR(100), -- Tensile, Impact, Chemical, etc.
    TestStandard NVARCHAR(100), -- EN 10002-1, EN 10045-1, etc.
    TestResults NVARCHAR(MAX), -- JSON format
    TestDate DATETIME2,
    TestLaboratory NVARCHAR(200)
);
```

#### Features
- Digital certificate management
- Automatic compliance verification
- Heat/batch tracking throughout production
- Material property lookup and validation

### 3. Welding Management System

#### WPS/WPQR Tracking
**Core Components:**
- Procedure library management
- Qualification record tracking
- Welder certification database
- Project-specific procedure assignment

**Database Schema:**
```sql
-- Welding Procedures Table
CREATE TABLE WeldingProcedures (
    WPSId UNIQUEIDENTIFIER PRIMARY KEY,
    WPSNumber NVARCHAR(50) UNIQUE,
    BaseMetalGroup NVARCHAR(50),
    WeldingProcess NVARCHAR(100),
    FillerMetal NVARCHAR(100),
    QualifiedThicknessRange NVARCHAR(50),
    QualifiedPositions NVARCHAR(100),
    QualificationDate DATETIME2,
    ExpiryDate DATETIME2,
    WPQRReference NVARCHAR(100),
    IsActive BIT
);

-- Welder Qualifications Table
CREATE TABLE WelderQualifications (
    QualificationId UNIQUEIDENTIFIER PRIMARY KEY,
    WelderId UNIQUEIDENTIFIER,
    WelderIdentification NVARCHAR(50),
    QualificationStandard NVARCHAR(100), -- EN ISO 9606-1
    WeldingProcess NVARCHAR(100),
    MaterialGroup NVARCHAR(50),
    ThicknessRange NVARCHAR(50),
    WeldingPositions NVARCHAR(100),
    QualificationDate DATETIME2,
    ExpiryDate DATETIME2,
    IsValid BIT
);
```

### 4. Inspection and NDT Management

#### Inspection Planning
**Automated Features:**
- Inspection requirement generation based on execution class
- NDT scheduling based on weld criticality
- Resource allocation and planning
- Quality hold point management

#### Digital Inspection Forms
**Functionality:**
- Customizable inspection checklists
- Photo documentation integration
- Digital signature capture
- Real-time compliance checking

**Implementation Example:**
```csharp
public class InspectionPlan
{
    public List<InspectionActivity> GenerateInspectionPlan(
        ProjectConfiguration config,
        List<WeldedConnection> connections)
    {
        var activities = new List<InspectionActivity>();
        
        foreach (var connection in connections)
        {
            // Add visual inspection (always required)
            activities.Add(new VisualInspection(connection));
            
            // Add NDT based on execution class
            switch (config.ExecutionClass)
            {
                case ExecutionClass.EXC3:
                case ExecutionClass.EXC4:
                    activities.Add(new UltrasonicTesting(connection));
                    activities.Add(new MagneticParticleTesting(connection));
                    break;
                case ExecutionClass.EXC2:
                    if (connection.IsCritical)
                        activities.Add(new UltrasonicTesting(connection));
                    break;
            }
        }
        
        return activities;
    }
}
```

### 5. Compliance Reporting and Documentation

#### Document Generation
**Automated Reports:**
- Declaration of Performance (DoP)
- CE marking documentation
- Manufacturing dossiers
- Inspection certificates
- Material traceability reports

#### Compliance Dashboard
**Key Metrics:**
- Execution class compliance status
- Outstanding inspection activities
- Certificate expiration tracking
- Non-conformance statistics
- Audit readiness indicators

### 6. Integration with Existing Features

#### Enhanced Inventory Management
- Link execution class requirements to material specifications
- Automatic material certificate validation
- Compliance checking during material allocation

#### Project Management Integration
- Execution class determination workflow
- Compliance milestone tracking
- Resource planning for inspection activities
- Document package preparation

#### User Interface Enhancements
**Navigation Structure:**
```
Steel Construction Compliance
├── Project Configuration
│   ├── Execution Class Determination
│   ├── Requirement Matrix
│   └── Compliance Planning
├── Material Compliance
│   ├── Certificate Management
│   ├── Traceability Tracking
│   └── Material Testing
├── Welding Management
│   ├── WPS/WPQR Library
│   ├── Welder Qualifications
│   └── Welding Coordination
├── Inspection & NDT
│   ├── Inspection Planning
│   ├── NDT Scheduling
│   └── Quality Documentation
└── Compliance Reporting
    ├── DoP Generation
    ├── CE Marking
    └── Audit Reports
```

---

## Practical Implementation Steps

### Phase 1: Foundation (Immediate Implementation)
1. **Database Schema Extension**
   - Add compliance-related tables to tenant databases
   - Implement material certificate storage
   - Create execution class configuration tables

2. **Basic Compliance Module**
   - Execution class determination wizard
   - Requirement matrix generation
   - Basic compliance dashboard

3. **Material Certificate Management**
   - Digital certificate upload and storage
   - Basic traceability linking
   - Certificate verification workflow

### Phase 2: Welding and Inspection (Next 3-6 months)
1. **Welding Management System**
   - WPS/WPQR database and tracking
   - Welder qualification management
   - Procedure assignment to projects

2. **Inspection Planning Module**
   - Automated inspection requirement generation
   - NDT scheduling and tracking
   - Digital inspection forms

3. **Quality Documentation**
   - Inspection report generation
   - Non-conformance tracking
   - Corrective action management

### Phase 3: Advanced Compliance (6-12 months)
1. **Full Document Generation**
   - Automated DoP creation
   - CE marking documentation
   - Manufacturing dossier compilation

2. **Advanced Analytics**
   - Compliance trend analysis
   - Audit preparation tools
   - Performance metrics dashboard

3. **Integration Enhancements**
   - ERP system integration
   - Customer portal for compliance documents
   - Regulatory reporting automation

### Implementation Considerations

#### Data Migration
- Plan for existing project data integration
- Define data mapping strategies
- Implement gradual rollout approach

#### User Training
- Develop training materials for EN 1090 requirements
- Create user guides for new compliance features
- Implement role-based access control

#### Performance Optimization
- Index critical lookup tables for compliance data
- Implement caching for frequently accessed documents
- Optimize database queries for large document sets

#### Security and Compliance
- Ensure document integrity with digital signatures
- Implement audit trails for all compliance activities
- Secure storage of sensitive certification data

---

## References and Standards

### Primary Standards
- **EN 1090-1:2009+A1:2011** - Execution of steel structures and aluminium structures - Part 1: Requirements for conformity assessment of structural components
- **EN 1090-2:2018** - Execution of steel structures and aluminium structures - Part 2: Technical requirements for steel structures
- **EN 1090-3:2008** - Execution of steel structures and aluminium structures - Part 3: Technical requirements for aluminium structures

### Supporting Standards

#### Welding Standards
- **EN ISO 9606-1:2017** - Qualification testing of welders - Fusion welding - Part 1: Steels
- **EN ISO 14732:2019** - Welding personnel - Qualification testing of welding operators and weld setters for mechanized and automatic welding of metallic materials
- **ISO 3834 (all parts)** - Quality requirements for fusion welding of metallic materials

#### NDT Standards
- **EN ISO 5817:2014** - Welding - Fusion-welded joints in steel, nickel, titanium and their alloys (beam welding excluded) - Quality levels for imperfections
- **EN ISO 17638:2016** - Non-destructive testing of welds - Magnetic particle testing
- **EN ISO 17640:2018** - Non-destructive testing of welds - Ultrasonic testing - Techniques, testing levels, and assessment
- **EN ISO 17636-1:2013** - Non-destructive testing of welds - Radiographic testing - X- and gamma-ray techniques with film

#### Material Standards
- **EN 10204:2004** - Metallic products - Types of inspection documents
- **EN 10025 (all parts)** - Hot rolled products of structural steels
- **EN 10210 (all parts)** - Hot finished structural hollow sections of non-alloy and fine grain steels
- **EN 10219 (all parts)** - Cold formed welded structural hollow sections of non-alloy and fine grain steels

#### Testing and Assessment
- **EN 10002-1:2001** - Metallic materials - Tensile testing - Part 1: Method of test at ambient temperature
- **EN 10045-1:1989** - Metallic materials - Charpy impact test - Part 1: Test method
- **EN ISO 9712:2012** - Non-destructive testing - Qualification and certification of NDT personnel

### Regulatory References
- **Construction Products Regulation (CPR) - Regulation (EU) No 305/2011**
- **EN 1993 (Eurocode 3)** - Design of steel structures
- **EN 1998 (Eurocode 8)** - Design of structures for earthquake resistance

### Guidance Documents
- **European Commission Guidance Papers** on Construction Products Regulation
- **ECCS (European Convention for Constructional Steelwork) Publications**
- **National Annexes** to EN 1090 (country-specific requirements)
- **CE Marking Guidelines** for construction products

### Online Resources
- **European Commission - Construction Products**: https://ec.europa.eu/growth/sectors/construction/product-regulation_en
- **CEN (European Committee for Standardization)**: https://www.cen.eu/
- **ECCS - European Convention for Constructional Steelwork**: https://www.steelconstruct.com/
- **Notified Bodies Database**: https://ec.europa.eu/growth/tools-databases/nando/

### Professional Organizations
- **BCSA (British Constructional Steelwork Association)**: Guidance on EN 1090 implementation
- **SCI (Steel Construction Institute)**: Technical guidance and training materials
- **NSSS (National Structural Steelwork Specification)**: UK implementation guidance

---

*This document serves as a comprehensive guide for implementing EN 1090 compliance within the Manimp steel construction management application. It should be regularly updated to reflect changes in standards and regulatory requirements.*

**Document Version:** 1.0  
**Last Updated:** January 2025  
**Next Review:** July 2025