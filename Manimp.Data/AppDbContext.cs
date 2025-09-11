using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Manimp.Auth.Models;
using Manimp.Shared.Models;

namespace Manimp.Data.Contexts;

/// <summary>
/// Database context for tenant-specific data including Identity and inventory management
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class
    /// </summary>
    /// <param name="options">The database context options</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Tier 1 Core Inventory DbSets
    /// <summary>
    /// Gets or sets the material types
    /// </summary>
    public DbSet<MaterialType> MaterialTypes { get; set; }

    /// <summary>
    /// Gets or sets the profile types
    /// </summary>
    public DbSet<ProfileType> ProfileTypes { get; set; }

    /// <summary>
    /// Gets or sets the steel grades
    /// </summary>
    public DbSet<SteelGrade> SteelGrades { get; set; }

    /// <summary>
    /// Gets or sets the suppliers
    /// </summary>
    public DbSet<Supplier> Suppliers { get; set; }

    /// <summary>
    /// Gets or sets the projects
    /// </summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>
    /// Gets or sets the documents
    /// </summary>
    public DbSet<Document> Documents { get; set; }

    /// <summary>
    /// Gets or sets the profile inventories
    /// </summary>
    public DbSet<ProfileInventory> ProfileInventories { get; set; }

    /// <summary>
    /// Gets or sets the profile usage logs
    /// </summary>
    public DbSet<ProfileUsageLog> ProfileUsageLogs { get; set; }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity tables to use shorter schema names if needed
        ConfigureIdentityModel(modelBuilder);

        // Apply Tier 1 Core Inventory Model Configuration
        ConfigureInventoryLookupTables(modelBuilder);
        ConfigureSupportingEntities(modelBuilder);
        ConfigureCoreInventoryEntities(modelBuilder);
    }

    /// <summary>
    /// Configures the Identity model extensions
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ConfigureIdentityModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });
    }

    /// <summary>
    /// Configures the inventory lookup tables (MaterialType, ProfileType, SteelGrade)
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ConfigureInventoryLookupTables(ModelBuilder modelBuilder)
    {
        // Configure MaterialType
        modelBuilder.Entity<MaterialType>(entity =>
        {
            entity.HasKey(e => e.MaterialTypeId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure ProfileType
        modelBuilder.Entity<ProfileType>(entity =>
        {
            entity.HasKey(e => e.ProfileTypeId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure SteelGrade
        modelBuilder.Entity<SteelGrade>(entity =>
        {
            entity.HasKey(e => e.SteelGradeId);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }

    /// <summary>
    /// Configures the supporting entities (Supplier, Project, Document)
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ConfigureSupportingEntities(ModelBuilder modelBuilder)
    {
        // Configure Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ContactInfo).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // Configure Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // Configure Document
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });
    }

    /// <summary>
    /// Configures the core inventory entities (ProfileInventory, ProfileUsageLog)
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ConfigureCoreInventoryEntities(ModelBuilder modelBuilder)
    {
        // Configure ProfileInventory
        modelBuilder.Entity<ProfileInventory>(entity =>
        {
            entity.HasKey(e => e.ProfileInventoryId);
            entity.Property(e => e.LotNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Size).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Length).HasColumnType("decimal(10,3)");
            entity.Property(e => e.WeightPerPiece).HasColumnType("decimal(10,2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            ConfigureProfileInventoryRelationships(entity);
            ConfigureProfileInventoryIndexes(entity);
        });

        // Configure ProfileUsageLog
        modelBuilder.Entity<ProfileUsageLog>(entity =>
        {
            entity.HasKey(e => e.ProfileUsageLogId);
            entity.Property(e => e.LengthUsed).HasColumnType("decimal(10,3)");
            entity.Property(e => e.Purpose).HasMaxLength(200);
            entity.Property(e => e.UsedBy).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            ConfigureProfileUsageLogRelationships(entity);
            ConfigureProfileUsageLogIndexes(entity);
        });
    }

    /// <summary>
    /// Configures the foreign key relationships for ProfileInventory
    /// </summary>
    /// <param name="entity">The entity type builder</param>
    private static void ConfigureProfileInventoryRelationships(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProfileInventory> entity)
    {
        // Foreign key relationships
        entity.HasOne(e => e.MaterialType)
              .WithMany(m => m.ProfileInventories)
              .HasForeignKey(e => e.MaterialTypeId)
              .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.ProfileType)
              .WithMany(p => p.ProfileInventories)
              .HasForeignKey(e => e.ProfileTypeId)
              .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.SteelGrade)
              .WithMany(s => s.ProfileInventories)
              .HasForeignKey(e => e.SteelGradeId)
              .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Supplier)
              .WithMany(s => s.ProfileInventories)
              .HasForeignKey(e => e.SupplierId)
              .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(e => e.CertificateDocument)
              .WithMany(d => d.ProfileInventories)
              .HasForeignKey(e => e.CertificateDocumentId)
              .OnDelete(DeleteBehavior.SetNull);
    }

    /// <summary>
    /// Configures the indexes for ProfileInventory
    /// </summary>
    /// <param name="entity">The entity type builder</param>
    private static void ConfigureProfileInventoryIndexes(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProfileInventory> entity)
    {
        // Indexes for performance
        entity.HasIndex(e => e.LotNumber);
        entity.HasIndex(e => new { e.MaterialTypeId, e.ProfileTypeId, e.SteelGradeId });
    }

    /// <summary>
    /// Configures the foreign key relationships for ProfileUsageLog
    /// </summary>
    /// <param name="entity">The entity type builder</param>
    private static void ConfigureProfileUsageLogRelationships(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProfileUsageLog> entity)
    {
        // Foreign key relationships
        entity.HasOne(e => e.ProfileInventory)
              .WithMany(p => p.ProfileUsageLogs)
              .HasForeignKey(e => e.ProfileInventoryId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Project)
              .WithMany(p => p.ProfileUsageLogs)
              .HasForeignKey(e => e.ProjectId)
              .OnDelete(DeleteBehavior.SetNull);
    }

    /// <summary>
    /// Configures the indexes for ProfileUsageLog
    /// </summary>
    /// <param name="entity">The entity type builder</param>
    private static void ConfigureProfileUsageLogIndexes(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProfileUsageLog> entity)
    {
        // Indexes for performance
        entity.HasIndex(e => e.UsedDate);
        entity.HasIndex(e => e.ProfileInventoryId);
    }
}
