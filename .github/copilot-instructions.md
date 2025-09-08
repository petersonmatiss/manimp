# Manimp - Metal Project Management System

Manimp is a .NET 8 Blazor Server application with MudBlazor UI for managing metal projects. It uses a database-per-tenant architecture with Azure SQL and ASP.NET Core Identity for authentication.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Prerequisites and Environment Setup
- Install .NET 8 SDK: `dotnet --version` should return 8.0.x
- Install Entity Framework tools globally: `dotnet tool install --global dotnet-ef`
- Verify EF tools: `dotnet ef --version`
- For Azure SQL development, ensure you have connection strings configured

### Repository Structure (as planned)
The solution follows this structure (see issue #1 for complete details):
```
Manimp.sln
├── Manimp.Shared/        - Common models and shared code
├── Manimp.Auth/          - Authentication and authorization
├── Manimp.Directory/     - Central directory for tenant mapping
├── Manimp.Data/          - Entity Framework contexts and models
├── Manimp.Services/      - Business logic and services
├── Manimp.Api/           - Web API controllers
└── Manimp.Web/           - Blazor Server UI with MudBlazor
```

### Initial Project Scaffolding
**WARNING: Repository is currently minimal. Use these commands to scaffold the complete solution:**

```bash
# Create solution file
dotnet new sln -n Manimp

# Create all projects
dotnet new classlib -n Manimp.Shared
dotnet new classlib -n Manimp.Auth
dotnet new classlib -n Manimp.Directory
dotnet new classlib -n Manimp.Data
dotnet new classlib -n Manimp.Services
dotnet new webapi -n Manimp.Api
dotnet new blazor -n Manimp.Web --interactivity Server --auth Individual

# Add projects to solution
dotnet sln add Manimp.Shared/Manimp.Shared.csproj
dotnet sln add Manimp.Auth/Manimp.Auth.csproj
dotnet sln add Manimp.Directory/Manimp.Directory.csproj
dotnet sln add Manimp.Data/Manimp.Data.csproj
dotnet sln add Manimp.Services/Manimp.Services.csproj
dotnet sln add Manimp.Api/Manimp.Api.csproj
dotnet sln add Manimp.Web/Manimp.Web.csproj
```

### Essential Package Dependencies
Add these packages to the appropriate projects:

```bash
# Web project - MudBlazor UI components (takes 30-60 seconds. NEVER CANCEL)
cd Manimp.Web
dotnet add package MudBlazor
# Set timeout to 120+ seconds

# Data projects - Entity Framework (takes 20-30 seconds each. NEVER CANCEL)
cd ../Manimp.Data
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
# Set timeout to 60+ seconds per command

cd ../Manimp.Directory
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools

# Auth project - Identity (takes 15-20 seconds. NEVER CANCEL)
cd ../Manimp.Auth
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
# Set timeout to 60+ seconds

# Optional - Logging
cd ../Manimp.Web
dotnet add package Serilog.AspNetCore
```

### Project References
Set up project dependencies as specified in issue #1:

```bash
# Manimp.Web dependencies
cd Manimp.Web
dotnet add reference ../Manimp.Services/Manimp.Services.csproj
dotnet add reference ../Manimp.Directory/Manimp.Directory.csproj
dotnet add reference ../Manimp.Data/Manimp.Data.csproj
dotnet add reference ../Manimp.Auth/Manimp.Auth.csproj
dotnet add reference ../Manimp.Shared/Manimp.Shared.csproj

# Manimp.Api dependencies (same as Web minus UI)
cd ../Manimp.Api
dotnet add reference ../Manimp.Services/Manimp.Services.csproj
dotnet add reference ../Manimp.Directory/Manimp.Directory.csproj
dotnet add reference ../Manimp.Data/Manimp.Data.csproj
dotnet add reference ../Manimp.Auth/Manimp.Auth.csproj
dotnet add reference ../Manimp.Shared/Manimp.Shared.csproj

# Manimp.Services dependencies
cd ../Manimp.Services
dotnet add reference ../Manimp.Directory/Manimp.Directory.csproj
dotnet add reference ../Manimp.Data/Manimp.Data.csproj
dotnet add reference ../Manimp.Auth/Manimp.Auth.csproj
dotnet add reference ../Manimp.Shared/Manimp.Shared.csproj

# Other project dependencies
cd ../Manimp.Data
dotnet add reference ../Manimp.Auth/Manimp.Auth.csproj
dotnet add reference ../Manimp.Shared/Manimp.Shared.csproj

cd ../Manimp.Auth
dotnet add reference ../Manimp.Shared/Manimp.Shared.csproj

cd ../Manimp.Directory
dotnet add reference ../Manimp.Shared/Manimp.Shared.csproj
```

### Build and Test Commands

#### Build the Solution
```bash
# Full solution build - Takes approximately 13 seconds. NEVER CANCEL.
dotnet build --configuration Release
# Set timeout to 30+ seconds for safety
```

#### Run Tests
```bash
# Run all tests - Takes approximately 2 seconds currently (no tests yet). NEVER CANCEL.
dotnet test
# Set timeout to 15+ seconds when tests are implemented
```

#### Restore Packages
```bash
# Restore all NuGet packages - Takes 15-30 seconds. NEVER CANCEL.
dotnet restore
# Set timeout to 60+ seconds for initial restore with many packages
```

### Running the Application

#### Development Mode (Blazor Server)
```bash
cd Manimp.Web
# Start development server - Usually starts in 5-10 seconds
dotnet run --urls http://localhost:5000
# App will be available at http://localhost:5000
# Use Ctrl+C to stop
```

#### Watch Mode for Development
```bash
cd Manimp.Web
# Hot reload during development - Takes 10-15 seconds to start. NEVER CANCEL.
dotnet watch run --urls http://localhost:5000
# Set timeout to 30+ seconds
```

### Database Operations

#### Entity Framework Migrations
```bash
# Add migration for Directory database (central)
cd Manimp.Directory
dotnet ef migrations add InitialCreate --context DirectoryDbContext

# Add migration for tenant database template
cd ../Manimp.Data
dotnet ef migrations add InitialCreate --context AppDbContext

# Apply migrations (requires valid connection strings)
dotnet ef database update --context DirectoryDbContext
dotnet ef database update --context AppDbContext
```

#### Database Connection Strings
Configure these in appsettings.json or user secrets (recommended for sensitive data):

```bash
# Initialize user secrets for the Web project
cd Manimp.Web
dotnet user-secrets init

# Set connection strings using user secrets (recommended)
dotnet user-secrets set "ConnectionStrings:Directory" "Server=localhost;Database=ManimpDirectory;Trusted_Connection=true;TrustServerCertificate=true;"
dotnet user-secrets set "ConnectionStrings:SqlServerAdmin" "Server=localhost;Trusted_Connection=true;TrustServerCertificate=true;"
dotnet user-secrets set "ConnectionStrings:TenantTemplate" "Server=localhost;Database={DB};Trusted_Connection=true;TrustServerCertificate=true;"

# For Azure SQL (production):
dotnet user-secrets set "ConnectionStrings:Directory" "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=ManimpDirectory;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

Required connection strings:
- `"Directory"` - Central directory database
- `"SqlServerAdmin"` - Admin connection for creating tenant databases  
- `"TenantTemplate"` - Template for tenant-specific connections (use {DB} placeholder)

### Validation and Quality Assurance

#### Before Committing Changes
**ALWAYS run these commands before committing:**

```bash
# Format code - Takes 15-20 seconds. NEVER CANCEL.
dotnet format
# Set timeout to 30+ seconds

# Build solution to catch compile errors - Takes 10-15 seconds. NEVER CANCEL.
dotnet build
# Set timeout to 30+ seconds

# Run tests (when implemented) - Currently takes 2 seconds. NEVER CANCEL.
dotnet test
# Set timeout to 60+ seconds when full test suite is implemented

# Check for common issues
dotnet build --verbosity normal
```

#### Manual Testing Scenarios
**ALWAYS test these scenarios after making changes:**

1. **Company Registration Flow:**
   - Navigate to registration page
   - Create new company with admin user
   - Verify tenant database creation
   - Verify admin can log in

2. **Login Flow:**
   - Test email-only login (no tenant selector)
   - Verify correct tenant resolution
   - Test failed login scenarios

3. **User Management (Admin):**
   - Log in as admin
   - Add new users to tenant
   - Verify user directory mapping

4. **Multi-tenant Isolation:**
   - Test that tenants cannot access each other's data
   - Verify database-per-tenant isolation

### Architecture-Specific Guidelines

#### Database-Per-Tenant Pattern
- Each tenant gets isolated SQL database
- Directory database maps email to tenant ID
- No cross-tenant data access
- Connection strings use template pattern: `"Server=...;Database={DB};..."`

#### Authentication Flow
- Email-only login (no tenant code required)
- System privately resolves tenant from email
- ASP.NET Core Identity stored in each tenant database
- No tenant information exposed in UI

#### Security Considerations
- GDPR-compliant data isolation
- No tenant enumeration possible
- Each tenant's data completely separated
- Audit all database access patterns

### Common Development Tasks

#### Adding New Features
1. Update models in Manimp.Shared
2. Add business logic in Manimp.Services  
3. Update database context in Manimp.Data
4. Create migration: `dotnet ef migrations add FeatureName`
5. Add UI components in Manimp.Web
6. Test multi-tenant scenarios

#### Troubleshooting Build Issues
- **Build fails with package restore errors**: Run `dotnet restore` and wait for completion (can take 60+ seconds)
- **Entity Framework command not found**: Install globally with `dotnet tool install --global dotnet-ef`
- **Project reference errors**: Check project references are correct using `dotnet list reference`
- **Package compatibility issues**: Verify all packages target .NET 8.0 framework
- **Connection string errors**: Use `dotnet user-secrets list` to verify secrets are set correctly
- **MudBlazor styling issues**: Ensure MudBlazor services are registered in Program.cs
- **Multi-tenant data issues**: Verify tenant resolution logic and database isolation
- **Memory issues during build**: Use `dotnet build -m:1` to limit parallel builds

#### Performance Considerations
- Monitor database connection pooling (especially with multiple tenants)
- Test with multiple concurrent tenants to validate isolation
- Validate migration performance on large databases (set timeout to 300+ seconds for migrations)
- Monitor memory usage with tenant switching
- Profile database queries for N+1 issues across tenants

## File Locations and Navigation

### Key Configuration Files
- `Manimp.Web/appsettings.json` - Main application configuration
- `Manimp.Web/Program.cs` - Application startup and service registration
- `Manimp.Data/AppDbContext.cs` - Tenant database context
- `Manimp.Directory/DirectoryDbContext.cs` - Central directory context
- `Manimp.Auth/` - Authentication and authorization logic

### Important Model Locations
- `Manimp.Shared/Models/` - Shared data models
- `Manimp.Data/Models/` - Entity Framework models
- `Manimp.Directory/Models/` - Directory-specific models (Tenants, UserDirectory)

### UI Components
- `Manimp.Web/Components/` - Blazor components
- `Manimp.Web/Pages/` - Page components
- `Manimp.Web/wwwroot/` - Static assets

## Validation Testing

### Quick Environment Validation
To validate your development environment is properly set up, run this quick test:

```bash
# Create test directory
mkdir /tmp/manimp-validate && cd /tmp/manimp-validate

# Test scaffolding - Takes 2-5 seconds. NEVER CANCEL.
dotnet new blazor -n TestApp --interactivity Server --auth Individual
# Set timeout to 30+ seconds

# Test build - Takes 4-8 seconds. NEVER CANCEL.
cd TestApp && dotnet build
# Set timeout to 30+ seconds

# Cleanup
cd /tmp && rm -rf manimp-validate

# If all commands succeed, your environment is ready
```

### Full Integration Validation
When implementing features, always test this complete scenario:

1. **Scaffold Complete Solution** (estimated 30-60 seconds total)
2. **Add All Package Dependencies** (estimated 2-5 minutes total)
3. **Build Solution** (estimated 10-15 seconds)
4. **Run Application** (estimated 5-10 seconds to start)
5. **Test Database Operations** (varies by database availability)

Remember: This application prioritizes data isolation, GDPR compliance, and secure multi-tenancy. Always validate tenant isolation when making changes to data access or authentication flows.