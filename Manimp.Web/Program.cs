using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using Manimp.Web.Components;
using Manimp.Directory.Data;
using Manimp.Data.Contexts;
using Manimp.Shared.Interfaces;
using Manimp.Services.Implementation;
using Manimp.Services.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add DbContext for Directory (commented out for demo)
// builder.Services.AddDbContext<DirectoryDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("Directory")));

// Register services (commented out for demo - requires database)
// builder.Services.AddScoped<ITenantService, TenantService>();
// builder.Services.AddScoped<ITenantDbContext, TenantDbContextService>();
// builder.Services.AddScoped<EN1090ProgressTrackingService>();

// Add health checks for container deployment (commented out for demo)
// builder.Services.AddHealthChecks()
//     .AddSqlServer(builder.Configuration.GetConnectionString("Directory")!, name: "directory_database");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add feature gate middleware (before routing and authorization) - disabled for demo
// app.UseFeatureGate();

app.UseAntiforgery();

// Add health check endpoints for container deployment (commented out for demo)
// app.MapHealthChecks("/health");
// app.MapHealthChecks("/health/ready");
// app.MapHealthChecks("/health/live");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Seed feature gating data on startup
// Disabled for demo - requires database connection
/*
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<FeatureGateDataSeeder>();
    await seeder.SeedInitialDataAsync();
}
*/

app.Run();
