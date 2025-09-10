using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Manimp.Auth.Models;
using Manimp.Data.Contexts;
using Manimp.Directory.Data;
using Manimp.Shared.Interfaces;
using Manimp.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext for Directory
builder.Services.AddDbContext<DirectoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Directory")));

builder.Services.AddDbContextFactory<AppDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddSignInManager();
builder.Services.AddAuthentication();

// Register services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICompanyRegistrationService, CompanyRegistrationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITenantDbContext, TenantDbContextService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();
