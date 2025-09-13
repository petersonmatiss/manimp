using Microsoft.EntityFrameworkCore;
using Manimp.Shared.Models;

namespace Manimp.Directory.Data;

public class DirectoryDbContext : DbContext
{
    public DirectoryDbContext(DbContextOptions<DirectoryDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<UserDirectory> UserDirectory { get; set; }
    
    // Feature Gating tables
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<PlanFeature> PlanFeatures { get; set; }
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
    public DbSet<TenantFeatureOverride> TenantFeatureOverrides { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.TenantId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DbName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SecretRef).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.DbName).IsUnique();
        });

        modelBuilder.Entity<UserDirectory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NormalizedEmail).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.NormalizedEmail);
            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure feature gating entities
        ConfigureFeatureGatingModel(modelBuilder);
    }

    /// <summary>
    /// Configures the feature gating model entities
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ConfigureFeatureGatingModel(ModelBuilder modelBuilder)
    {
        // Configure Plan
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.PlanId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.TierLevel);
        });

        // Configure Feature
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId);
            entity.Property(e => e.FeatureKey).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.HasIndex(e => e.FeatureKey).IsUnique();
            entity.HasIndex(e => e.Category);
        });

        // Configure PlanFeature
        modelBuilder.Entity<PlanFeature>(entity =>
        {
            entity.HasKey(e => e.PlanFeatureId);
            entity.HasOne(e => e.Plan)
                  .WithMany(p => p.PlanFeatures)
                  .HasForeignKey(e => e.PlanId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Feature)
                  .WithMany(f => f.PlanFeatures)
                  .HasForeignKey(e => e.FeatureId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.PlanId, e.FeatureId }).IsUnique();
        });

        // Configure TenantSubscription
        modelBuilder.Entity<TenantSubscription>(entity =>
        {
            entity.HasKey(e => e.TenantSubscriptionId);
            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Plan)
                  .WithMany(p => p.TenantSubscriptions)
                  .HasForeignKey(e => e.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.IsActive });
            entity.HasIndex(e => e.StartDate);
        });

        // Configure TenantFeatureOverride
        modelBuilder.Entity<TenantFeatureOverride>(entity =>
        {
            entity.HasKey(e => e.TenantFeatureOverrideId);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Feature)
                  .WithMany(f => f.TenantFeatureOverrides)
                  .HasForeignKey(e => e.FeatureId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.TenantId, e.FeatureId }).IsUnique();
            entity.HasIndex(e => e.ExpiresUtc);
        });
    }
}
