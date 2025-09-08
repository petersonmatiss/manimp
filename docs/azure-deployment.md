# Azure Deployment Guide for Manimp

This document provides comprehensive deployment instructions for the Manimp application on Azure, including cost optimization recommendations.

## Table of Contents

1. [CI/CD Pipeline](#cicd-pipeline)
2. [Azure App Service Deployment](#azure-app-service-deployment)
3. [Azure Container Apps Deployment (Recommended)](#azure-container-apps-deployment-recommended)
4. [Cost Comparison](#cost-comparison)
5. [Database Setup](#database-setup)
6. [Security Configuration](#security-configuration)
7. [Monitoring and Maintenance](#monitoring-and-maintenance)

## CI/CD Pipeline

### GitHub Actions Workflows

The repository includes three automated workflows:

1. **CI Workflow** (`.github/workflows/ci.yml`)
   - Triggers on push/PR to main/develop branches
   - Builds solution, runs tests, performs security scans
   - Publishes build artifacts for deployment

2. **Azure App Service Deployment** (`.github/workflows/deploy-azure.yml`)
   - Deploys to traditional Azure App Service
   - Supports staging and production environments
   - Automatic deployment from main branch

3. **Azure Container Apps Deployment** (`.github/workflows/deploy-container-apps.yml`)
   - Deploys to Azure Container Apps (recommended)
   - Containerized deployment with Docker
   - Better cost optimization and scaling

### Required GitHub Secrets

Configure these secrets in your GitHub repository:

#### For Azure App Service:
```
AZURE_WEBAPP_PUBLISH_PROFILE          # Download from Azure portal
AZURE_WEBAPP_PUBLISH_PROFILE_STAGING  # Download from Azure portal
AZURE_SQL_CONNECTION_STRING           # Production database connection
AZURE_SQL_CONNECTION_STRING_STAGING   # Staging database connection
```

#### For Azure Container Apps:
```
AZURE_CREDENTIALS                     # Service principal JSON
AZURE_CREDENTIALS_STAGING             # Staging service principal JSON
ACR_USERNAME                          # Azure Container Registry username
ACR_PASSWORD                          # Azure Container Registry password
AZURE_SQL_CONNECTION_STRING           # Production database connection
AZURE_SQL_CONNECTION_STRING_STAGING   # Staging database connection
AZURE_SQL_ADMIN_CONNECTION_STRING     # Admin connection for database creation
AZURE_SQL_ADMIN_CONNECTION_STRING_STAGING # Staging admin connection
AZURE_SQL_TENANT_TEMPLATE             # Template for tenant databases
AZURE_SQL_TENANT_TEMPLATE_STAGING     # Staging tenant template
```

## Azure App Service Deployment

### Prerequisites

1. **Azure Resource Group**
2. **Azure App Service Plan** (Standard tier or higher recommended)
3. **Azure SQL Database** (for Directory database)
4. **Azure Key Vault** (for secrets management)

### Step-by-Step Setup

#### 1. Create Azure Resources

```bash
# Login to Azure
az login

# Create resource group
az group create --name manimp-rg --location "East US"

# Create App Service Plan
az appservice plan create --name manimp-plan --resource-group manimp-rg --sku S1 --is-linux

# Create Web App
az webapp create --resource-group manimp-rg --plan manimp-plan --name manimp-app --runtime "DOTNETCORE:8.0"

# Create staging slot
az webapp deployment slot create --resource-group manimp-rg --name manimp-app --slot staging
```

#### 2. Configure Azure SQL Database

```bash
# Create SQL Server
az sql server create --name manimp-sql-server --resource-group manimp-rg --location "East US" --admin-user sqladmin --admin-password "YourStrongPassword123!"

# Create Directory database
az sql db create --resource-group manimp-rg --server manimp-sql-server --name ManimpDirectory --service-objective S0

# Configure firewall rules
az sql server firewall-rule create --resource-group manimp-rg --server manimp-sql-server --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
```

#### 3. Configure Application Settings

```bash
# Set connection strings
az webapp config connection-string set --resource-group manimp-rg --name manimp-app --connection-string-type SQLAzure --settings Directory="Server=tcp:manimp-sql-server.database.windows.net,1433;Initial Catalog=ManimpDirectory;Persist Security Info=False;User ID=sqladmin;Password=YourStrongPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Set app settings
az webapp config appsettings set --resource-group manimp-rg --name manimp-app --settings ASPNETCORE_ENVIRONMENT=Production
```

#### 4. Deploy Application

The GitHub Actions workflow will automatically deploy when you push to the main branch, or you can deploy manually:

```bash
# Build and publish locally
dotnet publish Manimp.Web/Manimp.Web.csproj -c Release -o ./publish

# Deploy using Azure CLI
az webapp deploy --resource-group manimp-rg --name manimp-app --src-path ./publish.zip --type zip
```

### App Service Pricing

**Standard S1 Plan:**
- **Cost**: ~$73/month
- **Features**: Custom domains, SSL, staging slots, auto-scale
- **Compute**: 1.75 GB RAM, 1 vCPU
- **Always-on pricing** (charges even when idle)

## Azure Container Apps Deployment (Recommended)

### Why Container Apps?

Azure Container Apps is recommended for Manimp because:

1. **Cost Efficiency**: Pay-per-use pricing, scales to zero
2. **Modern Platform**: Built for cloud-native applications
3. **Easy Scaling**: Automatic scaling based on demand
4. **Integration**: Built-in load balancing, SSL, monitoring

### Prerequisites

1. **Azure Container Registry** (for storing Docker images)
2. **Azure Container Apps Environment**
3. **Azure SQL Database**
4. **Azure Key Vault**

### Step-by-Step Setup

#### 1. Create Container Registry

```bash
# Create Azure Container Registry
az acr create --resource-group manimp-rg --name manimpregistry --sku Basic

# Enable admin user
az acr update --name manimpregistry --admin-enabled true

# Get login credentials
az acr credential show --name manimpregistry
```

#### 2. Create Container Apps Environment

```bash
# Install Container Apps extension
az extension add --name containerapp

# Create Container Apps environment
az containerapp env create --name manimp-env --resource-group manimp-rg --location "East US"
```

#### 3. Build and Push Docker Image

```bash
# Build Docker image
docker build -t manimpregistry.azurecr.io/manimp-web:latest .

# Login to ACR
az acr login --name manimpregistry

# Push image
docker push manimpregistry.azurecr.io/manimp-web:latest
```

#### 4. Deploy Container App

```bash
# Create container app
az containerapp create \
  --name manimp-app \
  --resource-group manimp-rg \
  --environment manimp-env \
  --image manimpregistry.azurecr.io/manimp-web:latest \
  --registry-server manimpregistry.azurecr.io \
  --registry-username $(az acr credential show --name manimpregistry --query username -o tsv) \
  --registry-password $(az acr credential show --name manimpregistry --query passwords[0].value -o tsv) \
  --target-port 8080 \
  --ingress external \
  --min-replicas 0 \
  --max-replicas 10 \
  --cpu 0.5 \
  --memory 1Gi
```

#### 5. Configure Environment Variables

```bash
# Set connection strings as secrets
az containerapp secret set --name manimp-app --resource-group manimp-rg --secrets directory-conn="Server=tcp:manimp-sql-server.database.windows.net,1433;Initial Catalog=ManimpDirectory;..."

# Update container app with environment variables
az containerapp update --name manimp-app --resource-group manimp-rg --set-env-vars ASPNETCORE_ENVIRONMENT=Production ConnectionStrings__Directory=secretref:directory-conn
```

### Container Apps Pricing

**Consumption Plan:**
- **Cost**: ~$15-30/month for typical usage
- **Features**: Auto-scaling, built-in load balancing, SSL
- **Compute**: 0.5 vCPU, 1 GB RAM per instance
- **Pay-per-use pricing** (no charges when idle)

## Cost Comparison

| Service | Monthly Cost | Pros | Cons | Best For |
|---------|-------------|------|------|----------|
| **App Service S1** | ~$73 | Easy setup, familiar platform | Always-on pricing, less efficient | Steady traffic, traditional deployments |
| **Container Apps** | ~$15-30 | Pay-per-use, modern platform, auto-scaling | Newer service, container complexity | Variable traffic, cost optimization |
| **App Service B1** | ~$15 | Lower cost than S1 | Limited features, no staging slots | Development/testing |

**Recommendation**: **Azure Container Apps** for production deployment due to:
- **60-80% cost savings** compared to App Service
- Better resource utilization
- Modern container-based architecture
- Automatic scaling to zero when not used

## Database Setup

### Multi-Tenant Database Architecture

Manimp uses a database-per-tenant architecture:

1. **Directory Database**: Central database for tenant mapping
2. **Tenant Databases**: Separate database for each company/tenant

#### Database Deployment Script

```sql
-- Directory Database Schema
CREATE TABLE Tenants (
    TenantId uniqueidentifier PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    DbName nvarchar(100) UNIQUE NOT NULL,
    SecretRef nvarchar(100),
    IsActive bit DEFAULT 1,
    CreatedUtc datetime2 DEFAULT GETUTCDATE()
);

CREATE TABLE UserDirectory (
    Id int IDENTITY(1,1) PRIMARY KEY,
    NormalizedEmail nvarchar(256) NOT NULL,
    TenantId uniqueidentifier NOT NULL,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);

CREATE INDEX IX_UserDirectory_NormalizedEmail ON UserDirectory(NormalizedEmail);
```

#### Connection String Configuration

For Azure SQL Database:

```json
{
  "ConnectionStrings": {
    "Directory": "Server=tcp:manimp-sql-server.database.windows.net,1433;Initial Catalog=ManimpDirectory;Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "SqlServerAdmin": "Server=tcp:manimp-sql-server.database.windows.net,1433;Persist Security Info=False;User ID={admin-username};Password={admin-password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "TenantTemplate": "Server=tcp:manimp-sql-server.database.windows.net,1433;Initial Catalog={DB};Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Security Configuration

### Azure Key Vault Integration

1. **Create Key Vault**:
```bash
az keyvault create --name manimp-keyvault --resource-group manimp-rg --location "East US"
```

2. **Store Secrets**:
```bash
az keyvault secret set --vault-name manimp-keyvault --name "Directory-ConnectionString" --value "Server=tcp:..."
```

3. **Configure Managed Identity**:
```bash
# Enable system-assigned managed identity
az webapp identity assign --resource-group manimp-rg --name manimp-app

# Grant Key Vault access
az keyvault set-policy --name manimp-keyvault --object-id {identity-principal-id} --secret-permissions get list
```

### Security Best Practices

1. **Use Managed Identities** for Azure service authentication
2. **Store sensitive data** in Azure Key Vault
3. **Enable HTTPS only** and TLS 1.2+
4. **Configure firewall rules** for Azure SQL
5. **Enable audit logging** for compliance
6. **Use private endpoints** for production databases

## Monitoring and Maintenance

### Application Insights

1. **Create Application Insights**:
```bash
az monitor app-insights component create --app manimp-insights --location "East US" --resource-group manimp-rg
```

2. **Get Instrumentation Key**:
```bash
az monitor app-insights component show --app manimp-insights --resource-group manimp-rg --query instrumentationKey
```

3. **Configure in application**:
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key"
  }
}
```

### Health Checks

The application includes health check endpoints:
- `/health` - Basic health check
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

### Database Maintenance

1. **Automated backups** are enabled by default
2. **Monitor database performance** using Azure SQL Analytics
3. **Set up alerts** for high CPU/memory usage
4. **Regular index maintenance** for tenant databases

### Scaling Configuration

#### Auto-scaling Rules (App Service)
```bash
az monitor autoscale create --resource-group manimp-rg --resource /subscriptions/{subscription-id}/resourceGroups/manimp-rg/providers/Microsoft.Web/serverfarms/manimp-plan --min-count 1 --max-count 5 --count 1
```

#### Container Apps Scaling
Container Apps automatically scale based on:
- HTTP requests
- CPU usage
- Memory usage
- Custom metrics

## Troubleshooting

### Common Issues

1. **Database Connection Failures**
   - Check firewall rules
   - Verify connection strings
   - Ensure managed identity permissions

2. **Application Startup Issues**
   - Check application logs
   - Verify environment variables
   - Check dependency injection configuration

3. **Performance Issues**
   - Monitor Application Insights
   - Check database query performance
   - Review scaling configuration

### Support Resources

- Azure support documentation
- Application logs via Azure portal
- Application Insights telemetry
- GitHub Actions workflow logs