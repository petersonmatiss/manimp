# Manimp - Metal Project Management System

Manimp is a .NET 8 Blazor Server application with MudBlazor UI for managing metal projects. It uses a database-per-tenant architecture with Azure SQL and ASP.NET Core Identity for authentication.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Prerequisites and Environment Setup
- Install .NET 8 SDK: `dotnet --version` should return 8.0.x
- Install Entity Framework tools globally: `dotnet tool install --global dotnet-ef`
- Verify EF tools: `dotnet ef --version`
- For Azure SQL development, ensure you have connection strings configured

### Repository Structure (current)
The solution follows this structure with complete implementation:
```
Manimp.sln
├── Manimp.Shared/        - Common models and shared code ✅
├── Manimp.Auth/          - Authentication and authorization ✅
├── Manimp.Directory/     - Central directory for tenant mapping ✅
├── Manimp.Data/          - Entity Framework contexts and models ✅
├── Manimp.Services/      - Business logic and services ✅
├── Manimp.Api/           - Web API controllers ✅
└── Manimp.Web/           - Blazor Server UI with MudBlazor ✅
```

### Working with the Existing Solution
**The complete solution is now scaffolded and ready for development. Key verification commands:**

```bash
# Verify solution structure
dotnet sln list

# Check project references for a specific project
cd Manimp.Web
dotnet list reference

# Verify all projects build
dotnet build --configuration Release
```

### Essential Package Dependencies
All required packages are already installed. Verify with these commands:

```bash
# Check Web project packages (MudBlazor, Serilog, etc.)
cd Manimp.Web
dotnet list package

# Check Data project packages (EF Core SqlServer and Tools)
cd ../Manimp.Data
dotnet list package

# Check Directory project packages (EF Core SqlServer and Tools)
cd ../Manimp.Directory
dotnet list package

# Check Auth project packages (Identity EntityFrameworkCore)
cd ../Manimp.Auth
dotnet list package

# Restore packages if needed (takes 15-30 seconds. NEVER CANCEL)
cd .. && dotnet restore
# Set timeout to 60+ seconds
```

### Project References
Project dependencies are already configured. Verify with these commands:

```bash
# Verify Manimp.Web dependencies
cd Manimp.Web
dotnet list reference

# Verify Manimp.Api dependencies  
cd ../Manimp.Api
dotnet list reference

# Verify Manimp.Services dependencies
cd ../Manimp.Services
dotnet list reference

# Verify all other project references
cd ../Manimp.Data && dotnet list reference
cd ../Manimp.Auth && dotnet list reference
cd ../Manimp.Directory && dotnet list reference

# If you need to add a new reference (example):
# dotnet add reference ../SomeProject/SomeProject.csproj
```

### Build and Test Commands

#### Build the Solution
```bash
#### Full solution build - Takes approximately 10-15 seconds. NEVER CANCEL.
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
Migrations are already created. To apply them or create new ones:

```bash
# Apply existing migrations for Directory database (central)
cd Manimp.Directory
dotnet ef database update --context DirectoryDbContext

# Apply existing migrations for tenant database template
cd ../Manimp.Data
dotnet ef database update --context AppDbContext

# Create new migrations when needed
cd Manimp.Directory
dotnet ef migrations add YourMigrationName --context DirectoryDbContext

cd ../Manimp.Data
dotnet ef migrations add YourMigrationName --context AppDbContext
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
To validate that the existing solution is working correctly:

```bash
# Verify solution structure
dotnet sln list

# Test build - Takes 10-15 seconds. NEVER CANCEL.
dotnet build
# Set timeout to 30+ seconds

# Run application in development mode
cd Manimp.Web && dotnet run --urls http://localhost:5000

# If all commands succeed, the solution is ready for development
```

### Full Integration Validation
When implementing features, always test this complete scenario on the existing solution:

1. **Build Solution** (estimated 10-15 seconds)
2. **Run Application** (estimated 5-10 seconds to start)
3. **Test Database Operations** (varies by database availability)
4. **Verify All Features Work** (company registration, login, user management)

Remember: This application prioritizes data isolation, GDPR compliance, and secure multi-tenancy. Always validate tenant isolation when making changes to data access or authentication flows.