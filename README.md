# Manimp

**One source of truth for metal projects**

Manimp is a multi-tenant metal project management application built with .NET 8, Blazor Server, and MudBlazor. It provides a secure, scalable platform for managing metal fabrication projects with complete tenant isolation.

## Architecture

### Technology Stack
- **.NET 8 (LTS)** - Core runtime and framework
- **Blazor Server + MudBlazor 8** - Modern, interactive web UI
- **Azure SQL** - Database-per-tenant architecture
- **ASP.NET Core Identity** - Authentication stored in each tenant DB
- **Serilog** - Structured logging

### Multi-Tenant Design
- **Database-per-tenant**: Each company gets its own isolated database
- **Private tenant resolution**: Email-only login with no tenant exposure
- **Central directory**: Maps normalized emails to tenant IDs securely
- **No shared data**: Complete isolation between tenants

## Project Structure

```
Manimp/
├── Manimp.Shared/          # Common models and interfaces (Models.cs)
├── Manimp.Auth/            # Authentication models (ApplicationUser.cs)
├── Manimp.Directory/       # Central directory service (DirectoryDbContext.cs)
├── Manimp.Data/            # Tenant database contexts (AppDbContext.cs)
├── Manimp.Services/        # Business logic services (TenantService.cs, CompanyRegistrationService.cs)
├── Manimp.Api/             # Web API endpoints
└── Manimp.Web/            # Blazor Server web application
```

## Phase 1 Features

### ✅ Available Now
- **Company Registration**: Creates tenant DB + Admin user
- **Email-Only Login**: Resolves tenant privately without exposure
- **User Management**: Admin can add users within tenant
- **Landing Page**: Feature overview and privacy information
- **Clean Architecture**: Template artifacts removed, proper file naming
- **Tier 1 Core Inventory Schema**: Baseline inventory management with lookup tables, profile tracking, and usage logging
- **Tier 2 Procurement and Remnants Module**: Advanced procurement tracking with POs, automated remnant creation, and full material lineage
- **Tier 3 Sourcing Module**: Price request and quote workflow for RFQ management and procurement planning
- **Feature Gating System**: Subscription-based feature access control with three-tier plans and override support
- **EN 1090 Compliance**: Steel construction manufacturing compliance system with subscription-tier based feature access, execution class validation, material traceability, and monthly project limits

### 🚧 Coming Next
- **Inventory UI**: User interface for managing profiles, materials, and usage
- **Procurement UI**: User interface for purchase orders and supplier management
- **Sourcing UI**: User interface for price requests and quote management
- **Projects**: Enhanced project management and tracking features
- **Remnants UI**: Interface for managing and utilizing remnant inventory
- **Production**: Manufacturing and quality control

## Database Schema

### Directory Database (Central)
```sql
Tenants
├── TenantId (uniqueidentifier, PK)
├── Name (nvarchar(200))
├── DbName (nvarchar(100), unique)
├── SecretRef (nvarchar(100))
├── IsActive (bit)
└── CreatedUtc (datetime2)

UserDirectory
├── Id (int, PK, identity)
├── NormalizedEmail (nvarchar(256), indexed)
└── TenantId (uniqueidentifier, FK)

Plans
├── PlanId (int, PK, identity)
├── Name (nvarchar(100), unique)
├── Description (nvarchar(500))
├── TierLevel (int)
├── IsActive (bit)
└── CreatedUtc (datetime2)

Features
├── FeatureId (int, PK, identity)
├── FeatureKey (nvarchar(100), unique)
├── Name (nvarchar(200))
├── Description (nvarchar(500))
├── Category (nvarchar(100))
├── IsActive (bit)
└── CreatedUtc (datetime2)

PlanFeatures
├── PlanFeatureId (int, PK, identity)
├── PlanId (int, FK → Plans)
├── FeatureId (int, FK → Features)
├── IsEnabled (bit)
└── CreatedUtc (datetime2)

TenantSubscriptions
├── TenantSubscriptionId (int, PK, identity)
├── TenantId (uniqueidentifier, FK → Tenants)
├── PlanId (int, FK → Plans)
├── StartDate (datetime2)
├── EndDate (datetime2, nullable)
├── IsActive (bit)
└── CreatedUtc (datetime2)

TenantFeatureOverrides
├── TenantFeatureOverrideId (int, PK, identity)
├── TenantId (uniqueidentifier, FK → Tenants)
├── FeatureId (int, FK → Features)
├── IsEnabled (bit)
├── Reason (nvarchar(500))
├── ExpiresUtc (datetime2, nullable)
└── CreatedUtc (datetime2)
```

### Tenant Database (Per Company)
- Standard ASP.NET Core Identity tables
- ApplicationUser extends IdentityUser with FirstName, LastName, CreatedUtc

#### Tier 1 Core Inventory Schema
**Lookup Tables:**
- **MaterialTypes**: Steel material classifications (structural, plate, etc.)
- **ProfileTypes**: Profile shapes (W-beam, channel, angle, etc.) 
- **SteelGrades**: Steel specifications (A992, A36, A572-50, etc.)

**Core Inventory:**
- **ProfileInventory**: Individual lots/sticks with pieces tracking
  - LotNumber, Size (e.g. W12x26), Length, PiecesOnHand, OriginalPieces
  - WeightPerPiece, UnitCost, ReceivedDate, Location, Notes
  - Links to MaterialType, ProfileType, SteelGrade, Supplier, Certificate
  - RowVersion for concurrency control
- **ProfileUsageLog**: Usage tracking and inventory decrements
  - PiecesUsed, LengthUsed, UsedDate, Purpose, UsedBy, Notes
  - Links to ProfileInventory and optional Project
  - Automatic decrement from ProfileInventory.PiecesOnHand

**Supporting Entities:**
- **Suppliers**: Vendor information for tagging lots
- **Projects**: Basic project info for usage tracking
- **Documents**: Certificate storage (mill certs, test reports, etc.)

**Features:**
- Optimistic concurrency with RowVersion
- Restrict deletions on lookup tables to preserve data integrity
- Cascade delete usage logs when inventory is deleted
- Forward-compatible schema for Tier 2+ features (POs, remnants, pricing)

### Tier 2 Procurement and Remnants Module

**New Entities (Advanced Subscriptions):**
- **PurchaseOrder**: Procurement tracking with supplier links
  - PONumber, OrderDate, Expected/ActualDeliveryDate, TotalAmount, Status
  - Links to Supplier, contains multiple PurchaseOrderLines
  - Unique PO numbers with date-based indexing
- **PurchaseOrderLine**: Individual line items on purchase orders
  - LineNumber, Size, Length, Quantity, UnitPrice, LineTotal, Description
  - Links to PurchaseOrder, MaterialType, ProfileType, SteelGrade
  - Enables detailed procurement specifications
- **ProfileRemnantInventory**: Automated remnant tracking
  - RemnantLotNumber (auto-generated), RemainingLength, RemnantPieces
  - CreatedDate, Location, IsAvailable status
  - Links to OriginalProfileInventory and ProfileUsageLog that created it
  - Automatic creation when material usage leaves leftover length

**ProfileInventory Enhancements:**
- Added **PurchaseOrderId** (nullable): Links inventory to originating purchase order
- Added **PONumber** (nullable): Reference PO number for manual tracking  
- Added **ProjectId** (nullable): Direct project association for inventory items

**Procurement Lineage:**
- Full traceability: PurchaseOrder → ProfileInventory → ProfileUsageLog → ProfileRemnantInventory
- Procurement costs can be tracked through to final usage and remnants
- Material certificates can be linked at PO or inventory level

**Remnant Automation:**
- When ProfileUsageLog is created with partial material usage, ProfileRemnantInventory is automatically generated
- Remnants maintain full material specifications and traceability to original lot
- Available remnants can be used for future projects, reducing waste

### Tier 3 Sourcing Module

**New Entities (Full Subscriptions):**
- **PriceRequest**: Request for quotes (RFQ) management
  - RequestNumber, RequestDate, RequiredByDate, Status, Notes
  - Links to optional Supplier (for directed requests) and Project
  - Status workflow: Draft → Sent → Quoted → Completed/Cancelled
- **PriceRequestLine**: Individual line items on price requests
  - LineNumber, Size, Length, Quantity, Description
  - Links to PriceRequest, MaterialType, ProfileType, SteelGrade
  - Enables detailed material specifications for quoting
- **PriceQuote**: Supplier responses to price requests
  - QuoteNumber, QuoteDate, ExpirationDate, TotalAmount, Status, Notes
  - Links to PriceRequest and Supplier
  - Status workflow: Received → Under Review → Accepted/Rejected/Expired

**PurchaseOrderLine Enhancements:**
- Added **PriceRequestLineId** (nullable): Links PO lines to originating price request lines

**Sourcing Workflow:**
- Complete RFQ process: PriceRequest → PriceRequestLine → PriceQuote → PurchaseOrderLine
- Full traceability from initial material requirement through quote comparison to final procurement
- Multiple suppliers can quote on the same price request for competitive pricing
- Accepted quotes can be automatically converted to purchase orders

**Integration with Procurement:**
- Price request lines can be converted to purchase order lines for streamlined procurement
- Sourcing history is maintained for vendor performance analysis
- Quote comparison features enable cost optimization

## Feature Gating System

### Subscription Plans

The feature gating system controls access to inventory module tiers based on tenant subscription plans:

#### Basic Plan (Tier 1)
- **Core Inventory Management**: Basic inventory tracking and management
- **Profile Management**: Manage material profiles and specifications
- **Usage Tracking**: Track material usage and consumption
- **Material Lookups**: Access to material type, profile, and grade lookups

#### Professional Plan (Tier 2)
- **All Basic Plan features** plus:
- **Procurement Management**: Advanced procurement tracking and management
- **Purchase Orders**: Create and manage purchase orders
- **Remnant Tracking**: Automated tracking of material remnants
- **Procurement Reports**: Advanced reporting for procurement activities

#### Enterprise Plan (Tier 3)
- **All Professional Plan features** plus:
- **Sourcing Management**: Strategic sourcing and vendor management
- **Price Requests**: Request for quote (RFQ) management
- **Quote Management**: Manage vendor quotes and comparisons
- **Vendor Comparison**: Compare vendors and pricing across quotes
- **Sourcing Reports**: Advanced reporting for sourcing activities
- **Project Management**: Enhanced project management and tracking
- **Quality Control**: Quality control and inspection management
- **Production Tracking**: Manufacturing and production tracking

### Feature Override System

Individual tenants can have feature overrides for:
- **Trial Access**: Temporary access to higher-tier features
- **Custom Agreements**: Special feature access based on contracts
- **Beta Testing**: Early access to new features
- **Granular Control**: Enable/disable specific features regardless of plan

### Implementation

The feature gating system uses:
- **Central Directory Database**: Stores tenant subscriptions and feature definitions
- **Service Layer**: `IFeatureGate` service validates tenant access to features
- **Middleware**: `FeatureGateMiddleware` enforces feature access at the request level
- **UI Components**: Feature-aware components show/hide functionality based on tenant permissions

## EN 1090 Compliance System

The EN 1090 compliance system ensures structural steelwork projects meet European standards for execution of steel structures. This comprehensive module provides full traceability, quality control, and documentation management based on tenant subscription tiers.

### Subscription Tier System

EN 1090 features are gated by tenant subscription tiers, determining which execution classes and compliance features are available:

#### Basic Tier (Tier 1)
- **Allowed Execution Classes**: EXC1 only
- **Requirements**: Basic documentation, standard procedures
- **Certificates**: EN 10204 2.1 minimum (optional)
- **Features**: Basic inventory, simple project tracking
- **Use Cases**: Simple structural components, basic buildings

#### Professional Tier (Tier 2) 
- **Allowed Execution Classes**: EXC1, EXC2, EXC3
- **Requirements**: Enhanced documentation, batch tracking mandatory for EXC2-EXC3
- **Certificates**: EN 10204 3.1 minimum for EXC2-EXC3
- **Features**: Full inventory management, procurement features, advanced project tracking
- **Use Cases**: Bridges, industrial buildings, most commercial structures

#### Enterprise Tier (Tier 3)
- **Allowed Execution Classes**: All classes (EXC1, EXC2, EXC3, EXC4)
- **Requirements**: Complete traceability, country of origin tracking
- **Certificates**: EN 10204 3.2 required for EXC4
- **Features**: Full CRM, sourcing management, complete compliance suite, advanced analytics
- **Use Cases**: Petrochemical plants, seismic zones, critical infrastructure

### Subscription-Based Access Control

- Tenant subscription tier determines available execution classes
- Real-time validation prevents creating projects beyond subscription tier
- Material compliance requirements scale with subscription tier
- Feature access controlled through centralized feature gating system

### Material Traceability

**Enhanced ProfileInventory with EN 1090 fields:**
- `MaterialBatch` - Heat/batch number for full material lineage
- `MillTestCertificateNumber` - Mill test certificate reference
- `CertificateType` - EN 10204 certificate type (2.1, 2.2, 3.1, 3.2)
- `MaterialStandard` - Applicable standard (e.g., EN 10025-2)
- `ManufacturingRoute` - Production method (Hot Rolled, Cold Formed)
- `SurfaceCondition` - Material condition (As Rolled, Shot Blasted)
- `CountryOfOrigin` - Required for EXC4 projects
- `TraceabilityNotes` - Additional compliance documentation

### Project Management Features

**Enhanced Project model with compliance tracking:**
- `ExecutionClass` - EN 1090 execution class (EXC1-EXC4)
- `ProjectTier` - Automatically calculated from execution class
- `CreatedMonth` - For monthly project limit tracking

**Monthly Project Limits:**
- Base limit: 10 projects per month per tenant
- Addon purchases: Additional projects available for purchase
- Automatic tracking: `TenantProjectLimit` tracks usage by month
- Overflow protection: Project creation blocked when limit reached

### Compliance Validation

**Real-time validation services:**
- `IEN1090ComplianceService` - Validates material compliance for each subscription tier
- Automatic subscription tier-based requirement checking
- Certificate type validation against execution class requirements
- Missing data identification for compliance gaps

**Validation Rules:**
- **Basic Tier**: Minimal validation, basic documentation
- **Professional Tier**: Material batch required for EXC2-EXC3, certificate 3.1+ required
- **Enterprise Tier**: All fields required, certificate 3.2 mandatory, country of origin required

### User Interface

**Project Management (`/projects/en1090`):**
- Project creation with execution class selection
- Subscription tier requirement display and validation
- Monthly project limit tracking with addon purchase option
- Compliance requirements display for each tier

**Material Traceability (`/inventory/en1090`):**
- Enhanced material entry forms with all EN 1090 fields
- Real-time compliance validation for all tiers
- Material compliance indicators (✓/✗ for each tier)
- Batch filtering and certificate type filtering

### Database Schema

**New/Enhanced Tables:**
```sql
-- Enhanced Projects table
Projects
├── ExecutionClass (nvarchar(10)) -- EXC1, EXC2, EXC3, EXC4
├── ProjectTier (int, nullable) -- 1, 2, or 3
└── CreatedMonth (nvarchar(7)) -- YYYY-MM format

-- Enhanced ProfileInventories table  
ProfileInventories
├── MaterialBatch (nvarchar(100))
├── MillTestCertificateNumber (nvarchar(100))
├── CertificateType (nvarchar(10)) -- 2.1, 2.2, 3.1, 3.2
├── MaterialStandard (nvarchar(50))
├── ManufacturingRoute (nvarchar(50))
├── SurfaceCondition (nvarchar(50))
├── CountryOfOrigin (nvarchar(100))
└── TraceabilityNotes (nvarchar(2000))

-- New project limit tracking (Directory DB)
TenantProjectLimits
├── TenantProjectLimitId (int, PK, identity)
├── TenantId (uniqueidentifier, FK → Tenants)
├── Month (nvarchar(7)) -- YYYY-MM format
├── ProjectsCreated (int)
├── BaseLimit (int, default 10)
├── AddonProjects (int, default 0)
├── CreatedUtc (datetime2)
└── UpdatedUtc (datetime2)
```

### Implementation

The feature gating system is implemented through:

- **IFeatureGate Service**: Centralized feature access checking
- **RequireFeature Attribute**: Declarative API endpoint protection
- **FeatureGate Middleware**: Automatic middleware-based feature gating
- **Frontend Integration**: UI components show/hide based on feature access
- **Database-Driven**: All feature definitions stored in database
- **Tenant Isolation**: Complete separation of tenant feature access

### Usage Examples

#### API Controller Protection
```csharp
[HttpGet("purchase-orders")]
[RequireFeature(FeatureKeys.PurchaseOrders)]
public IActionResult GetPurchaseOrders()
{
    // Only accessible to Professional and Enterprise plans
}

[HttpGet("sourcing")]
[RequireFeature(FeatureKeys.SourcingManagement, HideWhenDisabled = true)]
public IActionResult GetSourcing()
{
    // Returns 404 for unauthorized tenants (hidden feature)
}
```

#### Service-Level Checking
```csharp
public async Task<bool> CanAccessFeature(Guid tenantId, string featureKey)
{
    return await _featureGate.IsFeatureEnabledAsync(tenantId, featureKey);
}
```

#### Frontend Feature Gating
```razor
@if (await FeatureGate.IsFeatureEnabledAsync(TenantId, FeatureKeys.PurchaseOrders))
{
    <MudButton>Purchase Orders</MudButton>
}
```

### Demonstration

A complete feature gating demonstration is available:
- **Console Demo**: Run `dotnet run` in `FeatureGatingDemo/` project
- **Web Demo**: Navigate to `/features` page in the web application
- **API Demo**: Test API endpoints with different tenant contexts

The demo shows how tenants with different subscription plans have varying access to inventory features, with override capabilities for custom requirements.

## Configuration

### Connection Strings
```json
{
  "ConnectionStrings": {
    "Directory": "Server=...;Database=ManimpDirectory;...",
    "SqlServerAdmin": "Server=...;Trusted_Connection=true;...",
    "TenantTemplate": "Server=...;Database={DB};..."
  }
}
```

## GDPR Compliance & Privacy

### Data Protection by Design
- **Database-per-tenant**: Complete data isolation
- **No tenant lists**: No enumerable tenant information
- **Private resolution**: Email-to-tenant mapping is internal only
- **Secure secrets**: Database credentials stored securely (implement Azure Key Vault)

### Privacy Principles
1. **Data Minimization**: Only collect necessary information
2. **Purpose Limitation**: Data used only for intended purposes
3. **Storage Limitation**: Implement retention policies
4. **Transparency**: Clear privacy policy and data usage
5. **Individual Rights**: Support for data portability and deletion

### Compliance Features
- Tenant data can be completely isolated and exported
- Database-per-tenant enables easy data deletion
- No cross-tenant data contamination possible
- Audit logging with Serilog for compliance tracking

## Operations

### CI/CD Pipeline

The repository includes comprehensive GitHub Actions workflows for automated build, test, and deployment:

#### Workflows
- **CI Pipeline** (`.github/workflows/ci.yml`)
  - Builds and tests on every push/PR to main/develop
  - Code quality checks with `dotnet format`
  - Security scanning with Trivy
  - Publishes build artifacts

- **Azure Deployment** (`.github/workflows/deploy-azure.yml`)
  - Automated deployment to Azure App Service
  - Supports staging and production environments
  - Database migration support

- **Container Apps Deployment** (`.github/workflows/deploy-container-apps.yml`)
  - Deploys to Azure Container Apps (recommended)
  - Builds and pushes Docker images
  - Better cost optimization

#### Required GitHub Secrets
```
# For App Service deployment
AZURE_WEBAPP_PUBLISH_PROFILE
AZURE_WEBAPP_PUBLISH_PROFILE_STAGING
AZURE_SQL_CONNECTION_STRING
AZURE_SQL_CONNECTION_STRING_STAGING

# For Container Apps deployment (recommended)
AZURE_CREDENTIALS
AZURE_CREDENTIALS_STAGING
ACR_USERNAME
ACR_PASSWORD
```

### Azure Deployment Options

#### Option 1: Azure App Service (Traditional)
- **Cost**: ~$73/month (S1 plan)
- **Best for**: Traditional deployments, steady traffic
- **Features**: Easy setup, staging slots, auto-scale

#### Option 2: Azure Container Apps (Recommended) 🌟
- **Cost**: ~$15-30/month (60-80% savings)
- **Best for**: Variable traffic, cost optimization
- **Features**: Pay-per-use, auto-scale to zero, modern platform

See [`docs/azure-deployment.md`](docs/azure-deployment.md) for complete deployment instructions.

### Deployment
1. Deploy central Directory database
2. Configure connection strings and secrets
3. Run Directory migrations: `dotnet ef database update`
4. Deploy application to hosting environment

### Database Management
- **Directory DB**: Single shared database for tenant directory
- **Tenant DBs**: Created automatically during company registration
- **Migrations**: Tenant DB schema managed via EF Core migrations
- **Backups**: Database-per-tenant enables granular backup strategies

### Monitoring
- Structured logging with Serilog
- Application metrics and health checks (implement)
- Database performance monitoring
- Security audit trails

### Security Considerations
- Implement Azure Key Vault for database secrets
- Use managed identities for cloud deployments
- Regular security updates and vulnerability scanning
- Network security groups and firewalls

#### Security Scanning & CI/CD Security

The repository includes comprehensive automated security scanning as part of the CI/CD pipeline:

##### 🛡️ Security Workflows

1. **Main CI Security Scan** (`.github/workflows/ci.yml`)
   - **Filesystem Scanning**: Detects vulnerabilities in code and dependencies
   - **Secret Detection**: Scans for hardcoded secrets and credentials
   - **Critical Failure Policy**: Pipeline fails on CRITICAL or HIGH severity vulnerabilities
   - **SARIF Upload**: Results uploaded to GitHub Security tab for tracking
   - **Required for Deployment**: Security scan must pass before artifacts can be published

2. **.NET Package Security** (`.github/workflows/dotnet-security.yml`)
   - **NuGet Vulnerability Scanning**: Checks for vulnerable package dependencies
   - **Deprecated Package Detection**: Identifies deprecated packages (informational)
   - **Outdated Package Reporting**: Reports outdated dependencies (informational)
   - **Weekly Scheduled Scans**: Runs automatically every Sunday at 2 AM UTC
   - **Build Failure**: Pipeline fails if vulnerable packages are detected

3. **Container Security Scanning** (`.github/workflows/deploy-container-apps.yml`)
   - **Docker Image Scanning**: Scans built container images for vulnerabilities
   - **Base Image Vulnerabilities**: Detects issues in Microsoft base images
   - **Required for Deployment**: Container security scan must pass before deployment
   - **Multi-Environment**: Scans applied to both staging and production deployments

##### 🔧 Trivy Scanner Configuration

- **Scan Types**: Filesystem, secrets, container images
- **Severity Levels**: CRITICAL, HIGH, MEDIUM (with different failure policies)
- **Exit Policies**: 
  - Filesystem scan: Fails on CRITICAL, HIGH, MEDIUM
  - Secret scan: Fails on any secrets detected
  - Container scan: Fails on CRITICAL, HIGH
- **Reporting**: SARIF format for GitHub Security integration + human-readable reports

##### 📊 Security Reports

Security scan results are available in multiple formats:
- **GitHub Security Tab**: SARIF results for code scanning alerts
- **Build Artifacts**: Human-readable reports for debugging
- **Job Summaries**: Quick overview in GitHub Actions UI
- **Pull Request Comments**: Security findings (when applicable)

##### 🚫 Security Policy

- **Zero Tolerance**: No critical vulnerabilities allowed in main/develop branches
- **Automated Enforcement**: CI/CD pipeline automatically blocks vulnerable code
- **Regular Scanning**: Weekly automated scans for new vulnerabilities
- **Dependency Management**: Proactive monitoring of package vulnerabilities
- **Container Security**: All deployed images must pass security scans

##### 🔄 Vulnerability Response

1. **Detection**: Automated scans detect vulnerabilities
2. **Blocking**: CI/CD prevents deployment of vulnerable code
3. **Notification**: Developers notified via GitHub Security alerts
4. **Remediation**: Update vulnerable dependencies or apply security patches
5. **Verification**: Re-run security scans to confirm fixes

This multi-layered security approach ensures that both the codebase and deployed applications maintain high security standards throughout the development lifecycle.

## Development

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB for development)
- Visual Studio 2022 or VS Code

### Getting Started
```bash
# Clone repository
git clone https://github.com/petersonmatiss/manimp.git
cd manimp

# Restore packages
dotnet restore

# Run Directory migrations
cd Manimp.Directory
dotnet ef database update

# Run the web application
cd ../Manimp.Web
dotnet run
```

### Building
```bash
dotnet build
```

### Testing
```bash
dotnet test
```

## Support & Documentation

### EN 1090 Steel Construction Compliance

Comprehensive documentation for implementing EN 1090 compliance in steel construction manufacturing:

#### 📋 Compliance Documentation
- **[EN 1090 Compliance Guide](docs/en-1090-compliance.md)** - Complete overview of EN 1090 requirements, execution classes, welding standards, material traceability, and NDT requirements
- **[EN 1090 Development Guide](docs/en-1090-development.md)** - Technical implementation requirements, database schemas, API specifications, and UI components
- **[EN 1090 Quick Reference](docs/en-1090-quick-reference.md)** - Quick decision matrices, checklists, and common patterns for daily use

#### 🎯 Key Features Covered
- **Execution Classes (EXC1-EXC4)** - Automated determination based on project parameters
- **CE Marking Requirements** - Declaration of Performance (DoP) and compliance documentation
- **Material Traceability** - Certificate management and heat/batch tracking
- **Welding Management** - WPS/WPQR procedures and welder qualifications
- **Quality Control** - NDT planning, inspection management, and documentation
- **Factory Production Control (FPC)** - AVCP system integration and audit support

For technical support and detailed documentation, refer to:
- Application logs (structured with Serilog)
- Database schema documentation
- API documentation (Swagger in development)

## License

Copyright (c) 2025 Matiss Peterson. All rights reserved.