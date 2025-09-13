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

    // Tier 2 Procurement and Remnants DbSets
    /// <summary>
    /// Gets or sets the purchase orders
    /// </summary>
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    /// <summary>
    /// Gets or sets the purchase order lines
    /// </summary>
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }

    /// <summary>
    /// Gets or sets the profile remnant inventories
    /// </summary>
    public DbSet<ProfileRemnantInventory> ProfileRemnantInventories { get; set; }

    // Tier 3 Sourcing DbSets
    /// <summary>
    /// Gets or sets the price requests
    /// </summary>
    public DbSet<PriceRequest> PriceRequests { get; set; }

    /// <summary>
    /// Gets or sets the price request lines
    /// </summary>
    public DbSet<PriceRequestLine> PriceRequestLines { get; set; }

    /// <summary>
    /// Gets or sets the price quotes
    /// </summary>
    public DbSet<PriceQuote> PriceQuotes { get; set; }

    // CRM Module DbSets
    /// <summary>
    /// Gets or sets the customers
    /// </summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// Gets or sets the contacts
    /// </summary>
    public DbSet<Contact> Contacts { get; set; }

    /// <summary>
    /// Gets or sets the CRM projects
    /// </summary>
    public DbSet<CrmProject> CrmProjects { get; set; }

    /// <summary>
    /// Gets or sets the assembly lists
    /// </summary>
    public DbSet<AssemblyList> AssemblyLists { get; set; }

    /// <summary>
    /// Gets or sets the assemblies
    /// </summary>
    public DbSet<Assembly> Assemblies { get; set; }

    /// <summary>
    /// Gets or sets the parts
    /// </summary>
    public DbSet<Part> Parts { get; set; }

    /// <summary>
    /// Gets or sets the assembly parts
    /// </summary>
    public DbSet<AssemblyPart> AssemblyParts { get; set; }

    /// <summary>
    /// Gets or sets the bolts
    /// </summary>
    public DbSet<Bolt> Bolts { get; set; }

    /// <summary>
    /// Gets or sets the assembly part bolts
    /// </summary>
    public DbSet<AssemblyPartBolt> AssemblyPartBolts { get; set; }

    /// <summary>
    /// Gets or sets the coatings
    /// </summary>
    public DbSet<Coating> Coatings { get; set; }

    /// <summary>
    /// Gets or sets the assembly coatings
    /// </summary>
    public DbSet<AssemblyCoating> AssemblyCoatings { get; set; }

    /// <summary>
    /// Gets or sets the assembly outsourcing records
    /// </summary>
    public DbSet<AssemblyOutsourcing> AssemblyOutsourcings { get; set; }

    /// <summary>
    /// Gets or sets the cutting lists
    /// </summary>
    public DbSet<CuttingList> CuttingLists { get; set; }

    /// <summary>
    /// Gets or sets the cutting list entries
    /// </summary>
    public DbSet<CuttingListEntry> CuttingListEntries { get; set; }

    /// <summary>
    /// Gets or sets the deliveries
    /// </summary>
    public DbSet<Delivery> Deliveries { get; set; }

    /// <summary>
    /// Gets or sets the packing lists
    /// </summary>
    public DbSet<PackingList> PackingLists { get; set; }

    /// <summary>
    /// Gets or sets the packing list entries
    /// </summary>
    public DbSet<PackingListEntry> PackingListEntries { get; set; }

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

        // Apply Tier 2 Procurement and Remnants Model Configuration
        ApplyInventoryProcurementAndRemnantsModel(modelBuilder);

        // Apply Tier 3 Sourcing Model Configuration
        ApplyInventorySourcingModel(modelBuilder);

        // Apply CRM Module Model Configuration
        ApplyCrmModuleModel(modelBuilder);
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

            // Tier 2 Procurement and Project tracking properties
            entity.Property(e => e.PONumber).HasMaxLength(50);

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

        // Tier 2 Procurement and Project relationships
        entity.HasOne(e => e.PurchaseOrder)
              .WithMany(po => po.ProfileInventories)
              .HasForeignKey(e => e.PurchaseOrderId)
              .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(e => e.Project)
              .WithMany(p => p.ProfileInventories)
              .HasForeignKey(e => e.ProjectId)
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
        entity.HasIndex(e => e.ReceivedDate);
        entity.HasIndex(e => e.Location);
        entity.HasIndex(e => new { e.SupplierId, e.ReceivedDate });
        entity.HasIndex(e => new { e.PurchaseOrderId, e.ReceivedDate });
        entity.HasIndex(e => new { e.ProjectId, e.ReceivedDate });
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
        entity.HasIndex(e => new { e.ProjectId, e.UsedDate });
        entity.HasIndex(e => e.UsedBy);
    }

    /// <summary>
    /// Applies Tier 2 Procurement and Remnants model configuration
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ApplyInventoryProcurementAndRemnantsModel(ModelBuilder modelBuilder)
    {
        // Configure PurchaseOrder
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderId);
            entity.Property(e => e.PONumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            // Relationships
            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.PurchaseOrders)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(e => e.PONumber).IsUnique();
            entity.HasIndex(e => e.OrderDate);
        });

        // Configure PurchaseOrderLine
        modelBuilder.Entity<PurchaseOrderLine>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderLineId);
            entity.Property(e => e.Size).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Length).HasColumnType("decimal(10,3)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();

            // Relationships
            entity.HasOne(e => e.PurchaseOrder)
                  .WithMany(po => po.PurchaseOrderLines)
                  .HasForeignKey(e => e.PurchaseOrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.MaterialType)
                  .WithMany(mt => mt.PurchaseOrderLines)
                  .HasForeignKey(e => e.MaterialTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProfileType)
                  .WithMany(pt => pt.PurchaseOrderLines)
                  .HasForeignKey(e => e.ProfileTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SteelGrade)
                  .WithMany(sg => sg.PurchaseOrderLines)
                  .HasForeignKey(e => e.SteelGradeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PriceRequestLine)
                  .WithMany(prl => prl.PurchaseOrderLines)
                  .HasForeignKey(e => e.PriceRequestLineId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(e => new { e.PurchaseOrderId, e.LineNumber }).IsUnique();
        });

        // Configure ProfileRemnantInventory
        modelBuilder.Entity<ProfileRemnantInventory>(entity =>
        {
            entity.HasKey(e => e.ProfileRemnantInventoryId);
            entity.Property(e => e.RemnantLotNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.RemainingLength).HasColumnType("decimal(10,3)");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            // Relationships
            entity.HasOne(e => e.OriginalProfileInventory)
                  .WithMany(pi => pi.ProfileRemnantInventories)
                  .HasForeignKey(e => e.OriginalProfileInventoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProfileUsageLog)
                  .WithMany(pul => pul.ProfileRemnantInventories)
                  .HasForeignKey(e => e.ProfileUsageLogId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.RemnantLotNumber).IsUnique();
            entity.HasIndex(e => e.CreatedDate);
            entity.HasIndex(e => e.IsAvailable);
        });
    }

    /// <summary>
    /// Applies Tier 3 Sourcing model configuration
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ApplyInventorySourcingModel(ModelBuilder modelBuilder)
    {
        // Configure PriceRequest
        modelBuilder.Entity<PriceRequest>(entity =>
        {
            entity.HasKey(e => e.PriceRequestId);
            entity.Property(e => e.RequestNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            // Relationships
            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.PriceRequests)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Project)
                  .WithMany(p => p.PriceRequests)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(e => e.RequestNumber).IsUnique();
            entity.HasIndex(e => e.RequestDate);
            entity.HasIndex(e => e.Status);
        });

        // Configure PriceRequestLine
        modelBuilder.Entity<PriceRequestLine>(entity =>
        {
            entity.HasKey(e => e.PriceRequestLineId);
            entity.Property(e => e.Size).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Length).HasColumnType("decimal(10,3)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();

            // Relationships
            entity.HasOne(e => e.PriceRequest)
                  .WithMany(pr => pr.PriceRequestLines)
                  .HasForeignKey(e => e.PriceRequestId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.MaterialType)
                  .WithMany(mt => mt.PriceRequestLines)
                  .HasForeignKey(e => e.MaterialTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProfileType)
                  .WithMany(pt => pt.PriceRequestLines)
                  .HasForeignKey(e => e.ProfileTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SteelGrade)
                  .WithMany(sg => sg.PriceRequestLines)
                  .HasForeignKey(e => e.SteelGradeId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => new { e.PriceRequestId, e.LineNumber }).IsUnique();
        });

        // Configure PriceQuote
        modelBuilder.Entity<PriceQuote>(entity =>
        {
            entity.HasKey(e => e.PriceQuoteId);
            entity.Property(e => e.QuoteNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            // Relationships
            entity.HasOne(e => e.PriceRequest)
                  .WithMany(pr => pr.PriceQuotes)
                  .HasForeignKey(e => e.PriceRequestId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.PriceQuotes)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.QuoteNumber);
            entity.HasIndex(e => e.QuoteDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.PriceRequestId, e.SupplierId });
        });
    }

    /// <summary>
    /// Applies CRM Module model configuration
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void ApplyCrmModuleModel(ModelBuilder modelBuilder)
    {
        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CompanyName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CustomerCode).HasMaxLength(50);
            entity.Property(e => e.BillingAddress).HasMaxLength(1000);
            entity.Property(e => e.DefaultDeliveryAddress).HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasIndex(e => e.CompanyName);
            entity.HasIndex(e => e.CustomerCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // Configure Contact
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Contacts)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.IsPrimary);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure CrmProject
        modelBuilder.Entity<CrmProject>(entity =>
        {
            entity.HasKey(e => e.CrmProjectId);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ProjectCode).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(1000);
            entity.Property(e => e.DeliveryRules).HasMaxLength(2000);
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProjectValue).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.CrmProjects)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.ProjectCode).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.PlannedDeliveryDate);
        });

        // Configure AssemblyList
        modelBuilder.Entity<AssemblyList>(entity =>
        {
            entity.HasKey(e => e.AssemblyListId);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.UploadFileName).HasMaxLength(255);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.CrmProject)
                  .WithMany(p => p.AssemblyLists)
                  .HasForeignKey(e => e.CrmProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CrmProjectId);
            entity.HasIndex(e => e.CreatedUtc);
        });

        // Configure Assembly
        modelBuilder.Entity<Assembly>(entity =>
        {
            entity.HasKey(e => e.AssemblyId);
            entity.Property(e => e.AssemblyMark).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Weight).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ProgressPercentage).HasColumnType("decimal(5,2)");
            entity.Property(e => e.ManufacturingNotes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.AssemblyList)
                  .WithMany(al => al.Assemblies)
                  .HasForeignKey(e => e.AssemblyListId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.AssemblyListId);
            entity.HasIndex(e => e.AssemblyMark);
            entity.HasIndex(e => e.ProgressPercentage);
            entity.HasIndex(e => e.ManufacturingStarted);
            entity.HasIndex(e => e.ManufacturingCompleted);
        });

        // Configure Part
        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId);
            entity.Property(e => e.PartNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Dimensions).HasMaxLength(100);
            entity.Property(e => e.Length).HasColumnType("decimal(10,3)");
            entity.Property(e => e.WeightPerPiece).HasColumnType("decimal(10,2)");
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.MaterialType)
                  .WithMany()
                  .HasForeignKey(e => e.MaterialTypeId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.ProfileType)
                  .WithMany()
                  .HasForeignKey(e => e.ProfileTypeId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.SteelGrade)
                  .WithMany()
                  .HasForeignKey(e => e.SteelGradeId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.PartNumber).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.MaterialTypeId, e.ProfileTypeId, e.SteelGradeId });
        });

        // Configure AssemblyPart
        modelBuilder.Entity<AssemblyPart>(entity =>
        {
            entity.HasKey(e => e.AssemblyPartId);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Assembly)
                  .WithMany(a => a.AssemblyParts)
                  .HasForeignKey(e => e.AssemblyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Part)
                  .WithMany(p => p.AssemblyParts)
                  .HasForeignKey(e => e.PartId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.AssemblyId);
            entity.HasIndex(e => e.PartId);
            entity.HasIndex(e => new { e.AssemblyId, e.PartId }).IsUnique();
        });

        // Configure Bolt
        modelBuilder.Entity<Bolt>(entity =>
        {
            entity.HasKey(e => e.BoltId);
            entity.Property(e => e.BoltSpec).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Diameter).HasColumnType("decimal(8,2)");
            entity.Property(e => e.Length).HasColumnType("decimal(8,2)");
            entity.Property(e => e.Grade).HasMaxLength(50);
            entity.Property(e => e.Finish).HasMaxLength(100);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasIndex(e => e.BoltSpec).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.Diameter, e.Length, e.Grade });
        });

        // Configure AssemblyPartBolt
        modelBuilder.Entity<AssemblyPartBolt>(entity =>
        {
            entity.HasKey(e => e.AssemblyPartBoltId);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.AssemblyPart)
                  .WithMany(ap => ap.AssemblyPartBolts)
                  .HasForeignKey(e => e.AssemblyPartId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Bolt)
                  .WithMany(b => b.AssemblyPartBolts)
                  .HasForeignKey(e => e.BoltId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.AssemblyPartId);
            entity.HasIndex(e => e.BoltId);
        });

        // Configure Coating
        modelBuilder.Entity<Coating>(entity =>
        {
            entity.HasKey(e => e.CoatingId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CoatingType).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.ThicknessMicrons).HasColumnType("decimal(8,2)");
            entity.Property(e => e.CostPerSquareMeter).HasColumnType("decimal(10,2)");
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Supplier)
                  .WithMany()
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CoatingType);
            entity.HasIndex(e => e.SupplierId);
        });

        // Configure AssemblyCoating
        modelBuilder.Entity<AssemblyCoating>(entity =>
        {
            entity.HasKey(e => e.AssemblyCoatingId);
            entity.Property(e => e.SurfaceArea).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Assembly)
                  .WithMany(a => a.AssemblyCoatings)
                  .HasForeignKey(e => e.AssemblyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Coating)
                  .WithMany(c => c.AssemblyCoatings)
                  .HasForeignKey(e => e.CoatingId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.AssemblyId);
            entity.HasIndex(e => e.CoatingId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.AppliedDate);
        });

        // Configure AssemblyOutsourcing
        modelBuilder.Entity<AssemblyOutsourcing>(entity =>
        {
            entity.HasKey(e => e.AssemblyOutsourcingId);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Cost).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Assembly)
                  .WithMany(a => a.AssemblyOutsourcings)
                  .HasForeignKey(e => e.AssemblyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Supplier)
                  .WithMany()
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.AssemblyId);
            entity.HasIndex(e => e.SupplierId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.SentDate);
            entity.HasIndex(e => e.ExpectedReturnDate);
        });

        // Configure CuttingList
        modelBuilder.Entity<CuttingList>(entity =>
        {
            entity.HasKey(e => e.CuttingListId);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.CrmProject)
                  .WithMany()
                  .HasForeignKey(e => e.CrmProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CrmProjectId);
            entity.HasIndex(e => e.GeneratedUtc);
        });

        // Configure CuttingListEntry
        modelBuilder.Entity<CuttingListEntry>(entity =>
        {
            entity.HasKey(e => e.CuttingListEntryId);
            entity.Property(e => e.CutLength).HasColumnType("decimal(10,3)");
            entity.Property(e => e.CompletedBy).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.CuttingList)
                  .WithMany(cl => cl.CuttingListEntries)
                  .HasForeignKey(e => e.CuttingListId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Part)
                  .WithMany(p => p.CuttingListEntries)
                  .HasForeignKey(e => e.PartId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Assembly)
                  .WithMany()
                  .HasForeignKey(e => e.AssemblyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SourceProfileInventory)
                  .WithMany()
                  .HasForeignKey(e => e.SourceProfileInventoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.CuttingListId);
            entity.HasIndex(e => e.PartId);
            entity.HasIndex(e => e.AssemblyId);
            entity.HasIndex(e => e.SourceProfileInventoryId);
            entity.HasIndex(e => e.CutSequence);
            entity.HasIndex(e => e.IsCompleted);
        });

        // Configure Delivery
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId);
            entity.Property(e => e.DeliveryNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DeliveryAddress).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DeliveryMethod).HasMaxLength(100);
            entity.Property(e => e.DeliveredBy).HasMaxLength(100);
            entity.Property(e => e.ReceivedBy).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.CrmProject)
                  .WithMany(p => p.Deliveries)
                  .HasForeignKey(e => e.CrmProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CrmProjectId);
            entity.HasIndex(e => e.DeliveryNumber).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PlannedDeliveryDate);
            entity.HasIndex(e => e.ActualDeliveryDate);
        });

        // Configure PackingList
        modelBuilder.Entity<PackingList>(entity =>
        {
            entity.HasKey(e => e.PackingListId);
            entity.Property(e => e.PackingListNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Purpose).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Destination).HasMaxLength(500);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Delivery)
                  .WithMany(d => d.PackingLists)
                  .HasForeignKey(e => e.DeliveryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.DeliveryId);
            entity.HasIndex(e => e.PackingListNumber).IsUnique();
            entity.HasIndex(e => e.Purpose);
        });

        // Configure PackingListEntry
        modelBuilder.Entity<PackingListEntry>(entity =>
        {
            entity.HasKey(e => e.PackingListEntryId);
            entity.Property(e => e.ItemDescription).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Weight).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Dimensions).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.PackingList)
                  .WithMany(pl => pl.PackingListEntries)
                  .HasForeignKey(e => e.PackingListId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Assembly)
                  .WithMany()
                  .HasForeignKey(e => e.AssemblyId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.AssemblyOutsourcing)
                  .WithMany(ao => ao.PackingListEntries)
                  .HasForeignKey(e => e.AssemblyOutsourcingId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.PackingListId);
            entity.HasIndex(e => e.AssemblyId);
            entity.HasIndex(e => e.AssemblyOutsourcingId);
        });
    }
}
