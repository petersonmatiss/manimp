using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using Serilog;
using Manimp.Web.Components;
using Manimp.Directory.Data;
using Manimp.Data.Contexts;
using Manimp.Auth.Models;
using Manimp.Shared.Interfaces;
using Manimp.Services.Implementation;

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

// Add DbContext for Directory
builder.Services.AddDbContext<DirectoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Directory")));

// Add a dummy tenant DbContext for Identity registration (this will be replaced per-request)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DummyTenant;Trusted_Connection=true;MultipleActiveResultSets=true"));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    
    // Configure user requirements
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Register services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICompanyRegistrationService, CompanyRegistrationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITenantDbContext, TenantDbContextService>();

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
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
