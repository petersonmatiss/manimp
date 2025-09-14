using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Represents a customer for CRM and project management
/// </summary>
public class Customer
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer company name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer code/reference
    /// </summary>
    [MaxLength(50)]
    public string? CustomerCode { get; set; }

    /// <summary>
    /// Gets or sets the billing address
    /// </summary>
    [MaxLength(1000)]
    public string? BillingAddress { get; set; }

    /// <summary>
    /// Gets or sets the default delivery address
    /// </summary>
    [MaxLength(1000)]
    public string? DefaultDeliveryAddress { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether this customer is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the customer was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the contacts associated with this customer
    /// </summary>
    public ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();

    /// <summary>
    /// Gets the projects associated with this customer
    /// </summary>
    public ICollection<CrmProject> CrmProjects { get; set; } = new HashSet<CrmProject>();
}

/// <summary>
/// Represents a contact person for a customer
/// </summary>
public class Contact
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int ContactId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the job title
    /// </summary>
    [MaxLength(100)]
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    [MaxLength(50)]
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets whether this is the primary contact
    /// </summary>
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this contact is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when the contact was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Gets the full name of the contact
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// Extended project model for CRM with delivery rules and customer association
/// </summary>
public class CrmProject
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int CrmProjectId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the project name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project code/reference
    /// </summary>
    [MaxLength(50)]
    public string? ProjectCode { get; set; }

    /// <summary>
    /// Gets or sets the project description
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the delivery address for this project
    /// </summary>
    [MaxLength(1000)]
    public string? DeliveryAddress { get; set; }

    /// <summary>
    /// Gets or sets delivery rules and instructions
    /// </summary>
    [MaxLength(2000)]
    public string? DeliveryRules { get; set; }

    /// <summary>
    /// Gets or sets the project start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the planned delivery date
    /// </summary>
    public DateTime? PlannedDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the actual delivery date
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the project status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Planning";

    /// <summary>
    /// Gets or sets the project value
    /// </summary>
    public decimal? ProjectValue { get; set; }

    /// <summary>
    /// Gets or sets whether this project is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when the project was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Gets the assembly lists associated with this project
    /// </summary>
    public ICollection<AssemblyList> AssemblyLists { get; set; } = new HashSet<AssemblyList>();

    /// <summary>
    /// Gets the delivery records for this project
    /// </summary>
    public ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();
}

/// <summary>
/// Represents an assembly list for a project
/// </summary>
public class AssemblyList
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyListId { get; set; }

    /// <summary>
    /// Gets or sets the CRM project identifier
    /// </summary>
    public int CrmProjectId { get; set; }

    /// <summary>
    /// Gets or sets the assembly list name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assembly list description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the upload file name if imported from file
    /// </summary>
    [MaxLength(255)]
    public string? UploadFileName { get; set; }

    /// <summary>
    /// Gets or sets when the list was uploaded/created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated CRM project
    /// </summary>
    public CrmProject CrmProject { get; set; } = null!;

    /// <summary>
    /// Gets the assemblies in this list
    /// </summary>
    public ICollection<Assembly> Assemblies { get; set; } = new HashSet<Assembly>();
}

/// <summary>
/// Represents an assembly within an assembly list
/// </summary>
public class Assembly
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the assembly list identifier
    /// </summary>
    public int AssemblyListId { get; set; }

    /// <summary>
    /// Gets or sets the assembly mark/reference
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AssemblyMark { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assembly description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the quantity of this assembly
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the total weight of this assembly
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Gets or sets manufacturing progress percentage (0-100)
    /// </summary>
    public decimal? ProgressPercentage { get; set; } = 0;

    /// <summary>
    /// Gets or sets manufacturing notes
    /// </summary>
    [MaxLength(1000)]
    public string? ManufacturingNotes { get; set; }

    /// <summary>
    /// Gets or sets when manufacturing started
    /// </summary>
    public DateTime? ManufacturingStarted { get; set; }

    /// <summary>
    /// Gets or sets when manufacturing was completed
    /// </summary>
    public DateTime? ManufacturingCompleted { get; set; }

    /// <summary>
    /// Gets or sets the current EN 1090 progress status
    /// </summary>
    public AssemblyProgressStatus CurrentStatus { get; set; } = AssemblyProgressStatus.NotStarted;

    /// <summary>
    /// Gets or sets whether coating is outsourced for this assembly
    /// </summary>
    public bool IsCoatingOutsourced { get; set; } = false;

    /// <summary>
    /// Gets or sets when the assembly was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly list
    /// </summary>
    public AssemblyList AssemblyList { get; set; } = null!;

    /// <summary>
    /// Gets the parts that make up this assembly
    /// </summary>
    public ICollection<AssemblyPart> AssemblyParts { get; set; } = new HashSet<AssemblyPart>();

    /// <summary>
    /// Gets the coating records for this assembly
    /// </summary>
    public ICollection<AssemblyCoating> AssemblyCoatings { get; set; } = new HashSet<AssemblyCoating>();

    /// <summary>
    /// Gets the outsourcing records for this assembly
    /// </summary>
    public ICollection<AssemblyOutsourcing> AssemblyOutsourcings { get; set; } = new HashSet<AssemblyOutsourcing>();

    /// <summary>
    /// Gets the status history for this assembly
    /// </summary>
    public ICollection<AssemblyStatusHistory> StatusHistory { get; set; } = new HashSet<AssemblyStatusHistory>();

    /// <summary>
    /// Gets the quality assurance records for this assembly
    /// </summary>
    public ICollection<QualityAssuranceRecord> QualityAssuranceRecords { get; set; } = new HashSet<QualityAssuranceRecord>();

    /// <summary>
    /// Gets the non-compliance reports for this assembly
    /// </summary>
    public ICollection<NonComplianceReport> NonComplianceReports { get; set; } = new HashSet<NonComplianceReport>();

    /// <summary>
    /// Gets the outsourced coating tracking records for this assembly
    /// </summary>
    public ICollection<OutsourcedCoatingTracking> OutsourcedCoatingTrackings { get; set; } = new HashSet<OutsourcedCoatingTracking>();
}

/// <summary>
/// Represents a part used in assemblies
/// </summary>
public class Part
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PartId { get; set; }

    /// <summary>
    /// Gets or sets the part number/code
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the part description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the material type identifier
    /// </summary>
    public int? MaterialTypeId { get; set; }

    /// <summary>
    /// Gets or sets the profile type identifier
    /// </summary>
    public int? ProfileTypeId { get; set; }

    /// <summary>
    /// Gets or sets the steel grade identifier
    /// </summary>
    public int? SteelGradeId { get; set; }

    /// <summary>
    /// Gets or sets the part dimensions
    /// </summary>
    [MaxLength(100)]
    public string? Dimensions { get; set; }

    /// <summary>
    /// Gets or sets the length of the part
    /// </summary>
    public decimal? Length { get; set; }

    /// <summary>
    /// Gets or sets the weight per piece
    /// </summary>
    public decimal? WeightPerPiece { get; set; }

    /// <summary>
    /// Gets or sets whether this part is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the part was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated material type
    /// </summary>
    public MaterialType? MaterialType { get; set; }

    /// <summary>
    /// Gets the associated profile type
    /// </summary>
    public ProfileType? ProfileType { get; set; }

    /// <summary>
    /// Gets the associated steel grade
    /// </summary>
    public SteelGrade? SteelGrade { get; set; }

    /// <summary>
    /// Gets the assembly parts that use this part
    /// </summary>
    public ICollection<AssemblyPart> AssemblyParts { get; set; } = new HashSet<AssemblyPart>();

    /// <summary>
    /// Gets the cutting list entries for this part
    /// </summary>
    public ICollection<CuttingListEntry> CuttingListEntries { get; set; } = new HashSet<CuttingListEntry>();
}

/// <summary>
/// Links assemblies to their constituent parts
/// </summary>
public class AssemblyPart
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyPartId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the part identifier
    /// </summary>
    public int PartId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of this part needed for the assembly
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets additional notes for this part in the assembly
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this assembly part was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;

    /// <summary>
    /// Gets the associated part
    /// </summary>
    public Part Part { get; set; } = null!;

    /// <summary>
    /// Gets the bolts used for this assembly part
    /// </summary>
    public ICollection<AssemblyPartBolt> AssemblyPartBolts { get; set; } = new HashSet<AssemblyPartBolt>();
}

/// <summary>
/// Represents a bolt/fastener used in construction
/// </summary>
public class Bolt
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int BoltId { get; set; }

    /// <summary>
    /// Gets or sets the bolt specification
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string BoltSpec { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bolt description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the diameter
    /// </summary>
    public decimal? Diameter { get; set; }

    /// <summary>
    /// Gets or sets the length
    /// </summary>
    public decimal? Length { get; set; }

    /// <summary>
    /// Gets or sets the grade/strength
    /// </summary>
    [MaxLength(50)]
    public string? Grade { get; set; }

    /// <summary>
    /// Gets or sets the finish/coating
    /// </summary>
    [MaxLength(100)]
    public string? Finish { get; set; }

    /// <summary>
    /// Gets or sets whether this bolt is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the bolt was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the assembly part bolts that use this bolt
    /// </summary>
    public ICollection<AssemblyPartBolt> AssemblyPartBolts { get; set; } = new HashSet<AssemblyPartBolt>();
}

/// <summary>
/// Links assembly parts to their bolts
/// </summary>
public class AssemblyPartBolt
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyPartBoltId { get; set; }

    /// <summary>
    /// Gets or sets the assembly part identifier
    /// </summary>
    public int AssemblyPartId { get; set; }

    /// <summary>
    /// Gets or sets the bolt identifier
    /// </summary>
    public int BoltId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of this bolt needed
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this assembly part bolt was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly part
    /// </summary>
    public AssemblyPart AssemblyPart { get; set; } = null!;

    /// <summary>
    /// Gets the associated bolt
    /// </summary>
    public Bolt Bolt { get; set; } = null!;
}

/// <summary>
/// Represents a coating type that can be applied to assemblies
/// </summary>
public class Coating
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int CoatingId { get; set; }

    /// <summary>
    /// Gets or sets the coating name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the coating description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the coating type (Paint, Galvanizing, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? CoatingType { get; set; }

    /// <summary>
    /// Gets or sets the color
    /// </summary>
    [MaxLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the thickness in microns
    /// </summary>
    public decimal? ThicknessMicrons { get; set; }

    /// <summary>
    /// Gets or sets the supplier identifier
    /// </summary>
    public int? SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the cost per square meter
    /// </summary>
    public decimal? CostPerSquareMeter { get; set; }

    /// <summary>
    /// Gets or sets whether this coating is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the coating was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated supplier
    /// </summary>
    public Supplier? Supplier { get; set; }

    /// <summary>
    /// Gets the assembly coatings that use this coating
    /// </summary>
    public ICollection<AssemblyCoating> AssemblyCoatings { get; set; } = new HashSet<AssemblyCoating>();
}

/// <summary>
/// Links assemblies to their coatings
/// </summary>
public class AssemblyCoating
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyCoatingId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the coating identifier
    /// </summary>
    public int CoatingId { get; set; }

    /// <summary>
    /// Gets or sets the surface area to be coated
    /// </summary>
    public decimal? SurfaceArea { get; set; }

    /// <summary>
    /// Gets or sets the coating status
    /// </summary>
    [MaxLength(50)]
    public string? Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets when coating was applied
    /// </summary>
    public DateTime? AppliedDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this assembly coating was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;

    /// <summary>
    /// Gets the associated coating
    /// </summary>
    public Coating Coating { get; set; } = null!;
}

/// <summary>
/// Tracks outsourcing of assemblies or parts
/// </summary>
public class AssemblyOutsourcing
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int AssemblyOutsourcingId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the supplier identifier for outsourcing
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the outsourcing description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the outsourcing status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the cost for outsourcing
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Gets or sets when the work was sent out
    /// </summary>
    public DateTime? SentDate { get; set; }

    /// <summary>
    /// Gets or sets the expected return date
    /// </summary>
    public DateTime? ExpectedReturnDate { get; set; }

    /// <summary>
    /// Gets or sets the actual return date
    /// </summary>
    public DateTime? ActualReturnDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this outsourcing record was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;

    /// <summary>
    /// Gets the associated supplier
    /// </summary>
    public Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// Gets the packing list entries for this outsourcing
    /// </summary>
    public ICollection<PackingListEntry> PackingListEntries { get; set; } = new HashSet<PackingListEntry>();
}

/// <summary>
/// Represents optimized cutting lists for material usage
/// </summary>
public class CuttingList
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int CuttingListId { get; set; }

    /// <summary>
    /// Gets or sets the CRM project identifier
    /// </summary>
    public int CrmProjectId { get; set; }

    /// <summary>
    /// Gets or sets the cutting list name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets when the cutting list was generated
    /// </summary>
    public DateTime GeneratedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated CRM project
    /// </summary>
    public CrmProject CrmProject { get; set; } = null!;

    /// <summary>
    /// Gets the entries in this cutting list
    /// </summary>
    public ICollection<CuttingListEntry> CuttingListEntries { get; set; } = new HashSet<CuttingListEntry>();
}

/// <summary>
/// Individual entries in a cutting list with traceability
/// </summary>
public class CuttingListEntry
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int CuttingListEntryId { get; set; }

    /// <summary>
    /// Gets or sets the cutting list identifier
    /// </summary>
    public int CuttingListId { get; set; }

    /// <summary>
    /// Gets or sets the part identifier
    /// </summary>
    public int PartId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier this cut is for
    /// </summary>
    public int AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the source profile inventory identifier
    /// </summary>
    public int? SourceProfileInventoryId { get; set; }

    /// <summary>
    /// Gets or sets the quantity to cut
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the cut length
    /// </summary>
    public decimal CutLength { get; set; }

    /// <summary>
    /// Gets or sets the sequence order for cutting
    /// </summary>
    public int CutSequence { get; set; }

    /// <summary>
    /// Gets or sets whether this cut has been completed
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Gets or sets when the cut was completed
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the completed by user
    /// </summary>
    [MaxLength(100)]
    public string? CompletedBy { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this entry was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated cutting list
    /// </summary>
    public CuttingList CuttingList { get; set; } = null!;

    /// <summary>
    /// Gets the associated part
    /// </summary>
    public Part Part { get; set; } = null!;

    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly Assembly { get; set; } = null!;

    /// <summary>
    /// Gets the source profile inventory
    /// </summary>
    public ProfileInventory? SourceProfileInventory { get; set; }
}

/// <summary>
/// Represents a delivery to a customer
/// </summary>
public class Delivery
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int DeliveryId { get; set; }

    /// <summary>
    /// Gets or sets the CRM project identifier
    /// </summary>
    public int CrmProjectId { get; set; }

    /// <summary>
    /// Gets or sets the delivery reference number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DeliveryNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delivery address
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string DeliveryAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the planned delivery date
    /// </summary>
    public DateTime? PlannedDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the actual delivery date
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the delivery status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Planned";

    /// <summary>
    /// Gets or sets the delivery method/vehicle
    /// </summary>
    [MaxLength(100)]
    public string? DeliveryMethod { get; set; }

    /// <summary>
    /// Gets or sets the driver/delivery person
    /// </summary>
    [MaxLength(100)]
    public string? DeliveredBy { get; set; }

    /// <summary>
    /// Gets or sets who received the delivery
    /// </summary>
    [MaxLength(100)]
    public string? ReceivedBy { get; set; }

    /// <summary>
    /// Gets or sets delivery notes
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this delivery was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated CRM project
    /// </summary>
    public CrmProject CrmProject { get; set; } = null!;

    /// <summary>
    /// Gets the packing lists for this delivery
    /// </summary>
    public ICollection<PackingList> PackingLists { get; set; } = new HashSet<PackingList>();
}

/// <summary>
/// Represents a packing list for delivery or outsourcing
/// </summary>
public class PackingList
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PackingListId { get; set; }

    /// <summary>
    /// Gets or sets the delivery identifier (if for delivery)
    /// </summary>
    public int? DeliveryId { get; set; }

    /// <summary>
    /// Gets or sets the packing list number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string PackingListNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the purpose (Delivery, Outsourcing)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Purpose { get; set; } = "Delivery";

    /// <summary>
    /// Gets or sets the destination
    /// </summary>
    [MaxLength(500)]
    public string? Destination { get; set; }

    /// <summary>
    /// Gets or sets when the packing list was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated delivery
    /// </summary>
    public Delivery? Delivery { get; set; }

    /// <summary>
    /// Gets the entries in this packing list
    /// </summary>
    public ICollection<PackingListEntry> PackingListEntries { get; set; } = new HashSet<PackingListEntry>();
}

/// <summary>
/// Individual entries in a packing list
/// </summary>
public class PackingListEntry
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int PackingListEntryId { get; set; }

    /// <summary>
    /// Gets or sets the packing list identifier
    /// </summary>
    public int PackingListId { get; set; }

    /// <summary>
    /// Gets or sets the assembly identifier (if packing assemblies)
    /// </summary>
    public int? AssemblyId { get; set; }

    /// <summary>
    /// Gets or sets the assembly outsourcing identifier (if for outsourcing)
    /// </summary>
    public int? AssemblyOutsourcingId { get; set; }

    /// <summary>
    /// Gets or sets the item description
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string ItemDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the weight
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Gets or sets the dimensions
    /// </summary>
    [MaxLength(200)]
    public string? Dimensions { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when this entry was created
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    /// <summary>
    /// Gets the associated packing list
    /// </summary>
    public PackingList PackingList { get; set; } = null!;

    /// <summary>
    /// Gets the associated assembly
    /// </summary>
    public Assembly? Assembly { get; set; }

    /// <summary>
    /// Gets the associated assembly outsourcing
    /// </summary>
    public AssemblyOutsourcing? AssemblyOutsourcing { get; set; }
}