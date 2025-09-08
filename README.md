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

### 🚧 Coming Next
- **Projects**: Project management and tracking
- **Inventory**: Profiles, sheets, and materials management
- **Purchasing**: POs, invoices, and GRNs
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
```

### Tenant Database (Per Company)
- Standard ASP.NET Core Identity tables
- ApplicationUser extends IdentityUser with FirstName, LastName, CreatedUtc
- Future business tables (isolated per tenant)

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

For technical support and detailed documentation, refer to:
- Application logs (structured with Serilog)
- Database schema documentation
- API documentation (Swagger in development)

## License

Copyright (c) 2025 Matiss Peterson. All rights reserved.