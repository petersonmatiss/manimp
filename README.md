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
├── Manimp.Shared/          # Common models and interfaces
├── Manimp.Auth/            # Authentication models (ApplicationUser)
├── Manimp.Directory/       # Central directory service
├── Manimp.Data/            # Tenant database contexts
├── Manimp.Services/        # Business logic services
├── Manimp.Api/             # Web API endpoints
└── Manimp.Web/            # Blazor Server web application
```

## Phase 1 Features

### ✅ Available Now
- **Company Registration**: Creates tenant DB + Admin user
- **Email-Only Login**: Resolves tenant privately without exposure
- **User Management**: Admin can add users within tenant
- **Landing Page**: Feature overview and privacy information

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