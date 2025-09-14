using Microsoft.EntityFrameworkCore;
using Manimp.Directory.Data;
using Manimp.Shared.Interfaces;
using Manimp.Services;
using Manimp.Services.Implementation;
using Manimp.Services.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext for Directory
builder.Services.AddDbContext<DirectoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Directory")));

// Register services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICompanyRegistrationService, CompanyRegistrationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITenantDbContext, TenantDbContextService>();
builder.Services.AddScoped<IFeatureGate, FeatureGateService>();
builder.Services.AddScoped<IProjectLimitService, ProjectLimitService>();
builder.Services.AddScoped<IEN1090ComplianceService, EN1090ComplianceService>();
builder.Services.AddScoped<AssemblyProgressService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Add feature gate middleware (before controllers)
app.UseFeatureGate();

app.MapControllers();

app.Run();
