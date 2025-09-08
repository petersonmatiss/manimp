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
    }
}
